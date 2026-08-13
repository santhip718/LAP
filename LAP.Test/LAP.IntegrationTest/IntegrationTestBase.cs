using System.Net.Http.Headers;
using System.Net.Http.Json;
using LAP.Application.DTO.Auth;
using LAP.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }
    protected string AuthToken { get; private set; } = string.Empty;

    public IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {
        Factory.SeedDatabase();
        await AuthenticateAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task AuthenticateAsync()
    {
        using LearningAssessmentDbContext db = Factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = "Integration Test User",
            Email = $"inttest{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "1234567890",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage regResponse = await Client.PostJsonAsync(
            "/api/v1/auth/register", registerDto);

        if (regResponse.IsSuccessStatusCode)
        {
            AuthTokenResponseDto? result = await regResponse.Content
                .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
            if (result != null)
            {
                AuthToken = result.AccessToken;
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthToken);
            }
        }
    }

    protected async Task<string> RegisterAndGetTokenAsync(string? namePrefix = null)
    {
        using LearningAssessmentDbContext db = Factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = $"{namePrefix ?? "User"} {Guid.NewGuid():N}"[..30],
            Email = $"usr{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "1234567890",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage regResponse = await Client.PostJsonAsync(
            "/api/v1/auth/register", registerDto);

        if (regResponse.IsSuccessStatusCode)
        {
            AuthTokenResponseDto? result = await regResponse.Content
                .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
            return result?.AccessToken ?? string.Empty;
        }

        return string.Empty;
    }
}
