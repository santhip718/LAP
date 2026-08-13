using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest.Controller;

public class AuthControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        _factory.SeedDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<(string Email, string Password)> RegisterUserAsync(string? prefix = null)
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        string email = $"authtest{prefix ?? ""}{Guid.NewGuid():N}@example.com";
        const string password = "Password@123";

        var dto = new RegisterRequestDto
        {
            FullName = "Auth Test User",
            Email = email,
            Password = password,
            MobileNumber = "1234567890",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        await _client.PostJsonAsync("/api/v1/auth/register", dto);
        return (email, password);
    }

    // ─── Register ────────────────────────────────────────────────────

    [Fact]
    public async Task Register_ShouldReturnToken_WhenValid()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var dto = new RegisterRequestDto
        {
            FullName = "New User",
            Email = $"newuser{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "9876543210",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/register", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthTokenResponseDto? result = await response.Content
            .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.True(result.ExpiresIn > 0);
    }


    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailMissing()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var dto = new RegisterRequestDto
        {
            FullName = "No Email User",
            Email = "",
            Password = "Password@123",
            MobileNumber = "2222222222",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/register", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordTooShort()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var dto = new RegisterRequestDto
        {
            FullName = "Weak Password",
            Email = $"weak{Guid.NewGuid():N}@example.com",
            Password = "Ab1",
            MobileNumber = "3333333333",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/register", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─── Login ───────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsValid()
    {
        (string email, string password) = await RegisterUserAsync("login");

        var dto = new LoginRequestDto { Email = email, Password = password };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/login", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthTokenResponseDto? result = await response.Content
            .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenEmailDoesNotExist()
    {
        var dto = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "Password@123",
        };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/login", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordWrong()
    {
        (string email, _) = await RegisterUserAsync("wrongpw");

        var dto = new LoginRequestDto { Email = email, Password = "WrongPassword1!" };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/login", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenEmailEmpty()
    {
        var dto = new LoginRequestDto { Email = "", Password = "Password@123" };

        HttpResponseMessage response = await _client.PostJsonAsync("/api/v1/auth/login", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─── Refresh ─────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_ShouldReturnToken_WhenValidRefreshToken()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = "Refresh Test User",
            Email = $"refresh{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "4444444444",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage regResponse = await _client.PostJsonAsync(
            "/api/v1/auth/register", registerDto);
        AuthTokenResponseDto? regResult = await regResponse.Content
            .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(regResult);

        var refreshDto = new RefreshRequestDto { RefreshToken = regResult.RefreshToken };

        HttpResponseMessage response = await _client.PostJsonAsync(
            "/api/v1/auth/refresh", refreshDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthTokenResponseDto? result = await response.Content
            .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ShouldReturnBadRequest_WhenTokenEmpty()
    {
        var dto = new RefreshRequestDto { RefreshToken = "" };

        HttpResponseMessage response = await _client.PostJsonAsync(
            "/api/v1/auth/refresh", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenTokenInvalid()
    {
        var dto = new RefreshRequestDto { RefreshToken = "invalid-token-value" };

        HttpResponseMessage response = await _client.PostJsonAsync(
            "/api/v1/auth/refresh", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ─── Logout ──────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_ShouldReturnSuccess_WhenValidRefreshToken()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var designation = db.RefTerm.FirstOrDefault(x => x.Name == "Developer");
        var gender = db.RefTerm.FirstOrDefault(x => x.Name == "Male");

        var registerDto = new RegisterRequestDto
        {
            FullName = "Logout Test User",
            Email = $"logout{Guid.NewGuid():N}@example.com",
            Password = "Password@123",
            MobileNumber = "5555555555",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };

        HttpResponseMessage regResponse = await _client.PostJsonAsync(
            "/api/v1/auth/register", registerDto);
        AuthTokenResponseDto? regResult = await regResponse.Content
            .ReadFromJsonAsync<AuthTokenResponseDto>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(regResult);

        var logoutDto = new RefreshRequestDto { RefreshToken = regResult.RefreshToken };

        HttpResponseMessage response = await _client.PostJsonAsync(
            "/api/v1/auth/logout", logoutDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        SuccessResponse? result = await response.Content
            .ReadFromJsonAsync<SuccessResponse>(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task Logout_ShouldReturnSuccess_WhenTokenAlreadyRevoked()
    {
        var dto = new RefreshRequestDto { RefreshToken = "already-revoked-or-nonexistent" };

        HttpResponseMessage response = await _client.PostJsonAsync(
            "/api/v1/auth/logout", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
