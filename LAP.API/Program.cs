using System.Text;
using System.Text.Json;
using LAP.API.Extensions;
using LAP.API.Middleware;
using LAP.Application.DTO;
using LAP.Application.Helpers;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Infrastructure.Logging;
using LAP.Infrastructure.Persistence;
using LAP.Infrastructure.Persistence.SeedData;
using LAP.Infrastructure.Repository;
using LAP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

using System.Text.RegularExpressions;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console();
    },
    writeToProviders: true
);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

string rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
string connectionString = NormalizeConnectionString(rawConnectionString);

builder.Services.AddDbContext<LearningAssessmentDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

builder.Services.AddSwaggerDocumentation();

// Register services and repositories
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();

// File storage options
builder.Services.Configure<LAP.Application.Options.FileStorageOptions>(
    builder.Configuration.GetSection(LAP.Application.Options.FileStorageOptions.SectionName)
);

// CORS
string[] allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
                          ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});
var app = builder.Build();

// Auto-apply pending EF Core migrations on startup
if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", true))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();
        try
        {
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}

// Swagger
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger", true))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("FrontendPolicy");

app.UseStaticFiles();

// Register Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();
if (app.Configuration.GetValue<bool>("Seeding"))
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        LearningAssessmentDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        ReferenceDataSeeder seeder = new ReferenceDataSeeder(
            dbContext,
            scope.ServiceProvider.GetRequiredService<ICustomLogger<ReferenceDataSeeder>>()
        );
        await seeder.SeedAsync();

        AuthorizationDataSeeder authorizationSeeder = new AuthorizationDataSeeder(
            dbContext,
            scope.ServiceProvider.GetRequiredService<ICustomLogger<AuthorizationDataSeeder>>()
        );
        await authorizationSeeder.SeedAsync();

        UserDataSeeder userSeeder = new UserDataSeeder(
            dbContext,
            scope.ServiceProvider.GetRequiredService<ICustomLogger<UserDataSeeder>>()
        );
        await userSeeder.SeedAsync();
    }
}

// Authentication
app.UseAuthentication();

app.UseMiddleware<RequestContextMiddleware>();

// Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Explicit partial declaration to make the auto-generated Program class public,
/// enabling WebApplicationFactory&lt;Program&gt; in integration tests.
/// </summary>
public partial class Program
{
    public static string NormalizeConnectionString(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        string s = raw.Trim('"', '\'', ' ', '\r', '\n');

        // If user pasted postgresql:// or postgres:// URI format
        if (s.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var uri = new Uri(s);
                var userInfo = uri.UserInfo.Split(':');
                var db = uri.AbsolutePath.Trim('/');
                var csb = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port > 0 ? uri.Port : 5432,
                    Database = string.IsNullOrEmpty(db) ? "neondb" : db,
                    Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "neondb_owner",
                    Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true
                };
                return csb.ConnectionString;
            }
            catch { }
        }

        // Fix colons instead of equals (e.g., "Host: abc" -> "Host=abc")
        s = Regex.Replace(s, @"(?i)\bHost\s*[:]\s*", "Host=");
        s = Regex.Replace(s, @"(?i)\bServer\s*[:]\s*", "Server=");
        s = Regex.Replace(s, @"(?i)\bDatabase\s*[:]\s*", "Database=");
        s = Regex.Replace(s, @"(?i)\bUsername\s*[:]\s*", "Username=");
        s = Regex.Replace(s, @"(?i)\bPassword\s*[:]\s*", "Password=");
        s = Regex.Replace(s, @"(?i)\bUser\s*Id\s*[:]\s*", "User Id=");

        // If missing Host= or Server= at beginning (e.g. user pasted 'ep-little-math...; Database=...')
        if (!s.Contains("Host=", StringComparison.OrdinalIgnoreCase) &&
            !s.Contains("Server=", StringComparison.OrdinalIgnoreCase))
        {
            s = "Host=" + s;
        }

        // Strip unsupported libpq options like Channel Binding
        s = Regex.Replace(s, @"(?i)Channel\s+Binding\s*=\s*[^;]+;?", "");

        bool isLocal = s.Contains("Host=localhost", StringComparison.OrdinalIgnoreCase) ||
                       s.Contains("Host=127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                       s.Contains("Server=localhost", StringComparison.OrdinalIgnoreCase) ||
                       s.Contains("Server=127.0.0.1", StringComparison.OrdinalIgnoreCase);

        // Ensure SSL Mode and Trust Server Certificate for cloud connections
        if (!s.Contains("SSL Mode", StringComparison.OrdinalIgnoreCase) && !s.Contains("SslMode", StringComparison.OrdinalIgnoreCase))
        {
            s = s.TrimEnd(';') + (isLocal ? ";SSL Mode=Prefer;" : ";SSL Mode=Require;Trust Server Certificate=true;");
        }
        else if (!isLocal && !s.Contains("Trust Server Certificate", StringComparison.OrdinalIgnoreCase))
        {
            s = s.TrimEnd(';') + ";Trust Server Certificate=true;";
        }

        return s.Trim(' ', ';');
    }
}
