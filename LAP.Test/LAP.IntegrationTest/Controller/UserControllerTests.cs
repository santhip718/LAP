using System.Net;
using System.Net.Http.Json;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.User;
using LAP.IntegrationTest;

namespace LAP.Test.IntegrationTest.Controller;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string SeedUserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    private const string SeedUserEmail = "sathish@example.com";
    private static readonly Guid NonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public UserControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.SeedDatabase();

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("TestUserId", SeedUserId);
        _client.DefaultRequestHeaders.Add("TestUserEmail", SeedUserEmail);
        _client.DefaultRequestHeaders.Add("TestRole", "Admin");
    }

    // --- GET ALL ---

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenUsersExist()
    {
        var response = await _client.GetAsync("/api/v1/user");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedUsersDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        Assert.Contains(result.Data, u => u.Id == Guid.Parse(SeedUserId));
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedResult_WithCorrectPageSize()
    {
        var response = await _client.GetAsync("/api/v1/user?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedUsersDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.True(result.Total > 0);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/v1/user");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- GET BY ID ---

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenUserExists()
    {
        var response = await _client.GetAsync($"/api/v1/user/{SeedUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<UserEnrichedDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse(SeedUserId), result.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/v1/user/{NonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync("/api/v1/user/invalid-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync($"/api/v1/user/{SeedUserId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- GET PROFILE ---

    [Fact]
    public async Task GetProfile_ShouldReturnOk_WhenUserExists()
    {
        var response = await _client.GetAsync($"/api/v1/user/{SeedUserId}/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse(SeedUserId), result.Id);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/v1/user/{NonExistentId}/profile");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync($"/api/v1/user/{SeedUserId}/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- UPDATE ---

    [Fact]
    public async Task Update_ShouldReturnOk_WhenUserExists()
    {
        var dto = new UpdateUserRequestDto
        {
            FullName = "Sathish Updated",
            MobileNumber = "9876543211",
            DesignationId = TestSeedIds.RefTerm_SeniorDeveloper,
            GenderId = TestSeedIds.RefTerm_Male,
        };

        var response = await _client.PutAsync(
            $"/api/v1/user/{TestSeedIds.MutateUserId}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<UserDetailDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal("Sathish Updated", result.FullName);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var dto = new UpdateUserRequestDto
        {
            FullName = "Nobody",
            MobileNumber = "0000000000",
            DesignationId = TestSeedIds.RefTerm_JuniorDeveloper,
            GenderId = TestSeedIds.RefTerm_Male,
        };

        var response = await _client.PutAsync(
            $"/api/v1/user/{NonExistentId}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenFullNameIsEmpty()
    {
        var dto = new UpdateUserRequestDto
        {
            FullName = "",
            MobileNumber = "9876543210",
            DesignationId = TestSeedIds.RefTerm_JuniorDeveloper,
            GenderId = TestSeedIds.RefTerm_Male,
        };

        var response = await _client.PutAsync(
            $"/api/v1/user/{TestSeedIds.MutateUserId}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var dto = new UpdateUserRequestDto
        {
            FullName = "Sathish",
            MobileNumber = "9876543210",
            DesignationId = TestSeedIds.RefTerm_JuniorDeveloper,
            GenderId = TestSeedIds.RefTerm_Male,
        };

        var response = await unauthClient.PutAsync(
            $"/api/v1/user/{TestSeedIds.MutateUserId}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- DELETE ---

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenUserExists()
    {
        var response = await _client.DeleteAsync($"/api/v1/user/{TestSeedIds.DeleteUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.DeleteUserId, result.Id);
        Assert.Contains("deleted", result.Message.ToLower());
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/v1/user/{NonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.DeleteAsync($"/api/v1/user/{TestSeedIds.MutateUserId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
