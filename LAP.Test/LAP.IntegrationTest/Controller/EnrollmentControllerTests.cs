using System.Net;
using System.Net.Http.Json;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Enrollment;
using LAP.Application.DTO.Paginated;
using LAP.IntegrationTest;

namespace LAP.Test.IntegrationTest.Controller;

public class EnrollmentControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string SeedUserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    private const string SeedUserEmail = "sathish@example.com";
    private static readonly Guid NonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public EnrollmentControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAll_ShouldReturnOk_WhenEnrollmentsExist()
    {
        var response = await _client.GetAsync("/api/v1/enrollment");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedEnrollmentsDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        Assert.Contains(result.Data, e => e.Id == TestSeedIds.EnrollmentId_1);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAll_WhenAdmin()
    {
        var response = await _client.GetAsync("/api/v1/enrollment");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedEnrollmentsDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        Assert.Contains(result.Data, e => e.Id == TestSeedIds.EnrollmentId_1);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOwn_WhenStudent()
    {
        var studentClient = _factory.CreateClient();
        studentClient.DefaultRequestHeaders.Add("TestUserId", SeedUserId);
        studentClient.DefaultRequestHeaders.Add("TestUserEmail", SeedUserEmail);
        studentClient.DefaultRequestHeaders.Add("TestRole", "Student");

        var response = await studentClient.GetAsync("/api/v1/enrollment");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedEnrollmentsDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        Assert.All(result.Data, e => Assert.Equal(Guid.Parse(SeedUserId), e.UserId));
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/v1/enrollment");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- UPDATE ---

    [Fact]
    public async Task Update_ShouldReturnOk_WhenEnrollmentExists()
    {
        var dto = new UpdateEnrollmentRequestDto { EnrollmentStatus = false };

        var response = await _client.PutAsync(
            $"/api/v1/enrollment/{TestSeedIds.EnrollmentId_1}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.EnrollmentId_1, result.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenEnrollmentDoesNotExist()
    {
        var dto = new UpdateEnrollmentRequestDto { EnrollmentStatus = true };

        var response = await _client.PutAsync(
            $"/api/v1/enrollment/{NonExistentId}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var dto = new UpdateEnrollmentRequestDto { EnrollmentStatus = true };

        var response = await unauthClient.PutAsync(
            $"/api/v1/enrollment/{TestSeedIds.EnrollmentId_1}",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
