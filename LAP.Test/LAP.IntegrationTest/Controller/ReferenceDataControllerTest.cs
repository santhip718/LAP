using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest.Controller;

public class ReferenceDataControllerTest
    : IClassFixture<CustomWebApplicationFactory>,
        IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReferenceDataControllerTest(CustomWebApplicationFactory factory)
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
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = "RefData Test User",
            Email = $"refdata{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "1234567890",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage regResponse = await _client.PostJsonAsync(
            "/api/v1/auth/register",
            registerDto
        );

        if (regResponse.IsSuccessStatusCode)
        {
            AuthTokenResponseDto? result =
                await regResponse.Content.ReadFromJsonAsync<AuthTokenResponseDto>(
                    TestHelper.SnakeCaseOptions
                );
            if (result != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    result.AccessToken
                );
            }
        }
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnTerms_WhenRefSetExists()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/reference-data/Gender");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<RefTermDto>? result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, r => r.Name == "Male");
        Assert.Contains(result, r => r.Name == "Female");
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnEmpty_WhenRefSetDoesNotExist()
    {
        HttpResponseMessage response = await _client.GetAsync(
            "/api/v1/reference-data/NonExistentSet"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<RefTermDto>? result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnTerms_WhenRefSetNameIsCaseInsensitive()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/reference-data/gender");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<RefTermDto>? result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnTerms_ForCategory()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/reference-data/Category");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<RefTermDto>? result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.Contains(result, r => r.Name == "Technology");
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnOk_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync("/api/v1/reference-data/Gender");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetReferenceData_ShouldReturnTerms_ForDifficultyLevel()
    {
        HttpResponseMessage response = await _client.GetAsync(
            "/api/v1/reference-data/DifficultyLevel"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<RefTermDto>? result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.Contains(result, r => r.Name == "Beginner");
        Assert.Contains(result, r => r.Name == "Advanced");
    }
}
