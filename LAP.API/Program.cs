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

builder.Services.AddDbContext<LearningAssessmentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
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
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("FrontendPolicy");

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
public partial class Program { }
