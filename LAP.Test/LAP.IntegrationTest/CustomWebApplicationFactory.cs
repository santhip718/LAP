using LAP.Infrastructure.Persistence;
using LAP.IntegrationTest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LAP.Test.IntegrationTest;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString("N");
    private readonly string _fileStorageRoot = Path.Combine(
        Path.GetTempPath(),
        "LAP.IntegrationTest",
        Guid.NewGuid().ToString("N")
    );

    private readonly string _questionTemplatePath = Path.Combine(
        Path.GetTempPath(),
        "LAP.IntegrationTest",
        Guid.NewGuid().ToString("N")
    );

    public string QuestionTemplatePath => _questionTemplatePath;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "FileStorageOptions:StorageRoot",
            _fileStorageRoot
        );

        builder.UseSetting(
            "FileStorageOptions:QuestionTemplatePath",
            _questionTemplatePath
        );

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<LearningAssessmentDbContext>>();
            services.RemoveAll<LearningAssessmentDbContext>();

            services.AddDbContext<LearningAssessmentDbContext>(options =>
                options
                    .UseInMemoryDatabase(_databaseName)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            );

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "SmartAuth";
                    options.DefaultChallengeScheme = "SmartAuth";
                })
                .AddPolicyScheme(
                    "SmartAuth",
                    "SmartAuth",
                    options =>
                    {
                        options.ForwardDefaultSelector = context =>
                            context.Request.Headers.ContainsKey(TestAuthHandler.TestUserIdHeader)
                                ? TestAuthHandler.AuthScheme
                                : JwtBearerDefaults.AuthenticationScheme;
                    }
                )
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthScheme,
                    null
                );
        });
    }

    public LearningAssessmentDbContext CreateDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        LearningAssessmentDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    public void SeedDatabase()
    {
        using LearningAssessmentDbContext db = CreateDbContext();
        TestDataSeeder.Seed(db);
    }
}
