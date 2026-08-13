using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Auth;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest.Controller;

public class LeaderboardControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public LeaderboardControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        _factory.SeedDatabase();
        await AuthenticateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task AuthenticateAsync()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.First(x => x.Name == "Developer");
        var gender = db.RefTerm.First(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = "Leaderboard Test User",
            Email = $"leader{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "1234567890",
            DesignationId = designation.Id,
            GenderId = gender.Id,
        };

        HttpResponseMessage regResponse = await _client.PostJsonAsync("/api/v1/auth/register", registerDto);
        if (regResponse.IsSuccessStatusCode)
        {
            AuthTokenResponseDto? result = await regResponse.Content.ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
            if (result != null)
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.AccessToken);
            }
        }
    }

    [Fact]
    public async Task GetOverallPlatformLeaderboard_ShouldReturnOkAndList()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/leaderboard/overall");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<LeaderboardDto>? result = await response.Content
            .ReadFromJsonAsync<List<LeaderboardDto>>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetOverallPlatformLeaderboard_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync("/api/v1/leaderboard/overall");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
