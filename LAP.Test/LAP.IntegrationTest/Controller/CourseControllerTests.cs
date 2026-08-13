using System.Net;
using System.Net.Http.Json;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.Forum;
using LAP.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest.Controller;

public class CourseControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string SeedUserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    private const string SeedUserEmail = "sathish@example.com";
    private static readonly Guid NonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public CourseControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("TestUserId", SeedUserId);
        _client.DefaultRequestHeaders.Add("TestUserEmail", SeedUserEmail);
        _client.DefaultRequestHeaders.Add("TestRole", "Admin");

        _factory.SeedDatabase();
    }

    // --- DIAGNOSTIC ---

    [Fact]
    public async Task Diagnostic_CheckSeedData()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LAP.Infrastructure.Persistence.LearningAssessmentDbContext>();
        var courseCount = db.Course.Count();
        var refSetCount = db.RefSet.Count();
        var enrollmentCount = db.Enrollment.Count();
        var courseIds = db.Course.Select(c => c.Id).ToList();
        var courseTitles = db.Course.Select(c => c.Title).ToList();

        Assert.True(refSetCount > 0, $"RefSet count: {refSetCount} (0 = seed skipped or failed)");
        var diagnostic = $"Course count={courseCount}, RefSet={refSetCount}, Enrollments={enrollmentCount}, " +
            $"Course IDs=[{string.Join(", ", courseIds)}], " +
            $"Course titles=[{string.Join(", ", courseTitles)}], " +
            $"Has CourseId_1={courseIds.Contains(TestSeedIds.CourseId_1)}";
        Assert.True(courseCount > 0, diagnostic);
        Assert.True(
            db.Course.Any(c => c.Id == TestSeedIds.CourseId_1),
            $"CourseId_1 ({TestSeedIds.CourseId_1}) not found. {diagnostic}"
        );
    }

    // --- CREATE ---

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValidRequest()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Course"), "title");
        form.Add(new StringContent("Course description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("60"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await _client.PostAsync("/api/v1/course", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTitleIsMissing()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(""), "title");
        form.Add(new StringContent("Description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("60"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await _client.PostAsync("/api/v1/course", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Course"), "title");
        form.Add(new StringContent("Description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("60"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await unauthClient.PostAsync("/api/v1/course", form);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenThumbnailProvided()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Course With Thumbnail"), "title");
        form.Add(new StringContent("Description with image"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("45"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");
        var imageContent = new ByteArrayContent([0x89, 0x50, 0x4E, 0x47]);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        form.Add(imageContent, "thumbnail_img", "test.png");

        var response = await _client.PostAsync("/api/v1/course", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    // --- UPDATE ---

    [Fact]
    public async Task Update_ShouldReturnOk_WhenCourseExists()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Updated Course Title"), "title");
        form.Add(new StringContent("Updated description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("90"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await _client.PutAsync($"/api/v1/course/{TestSeedIds.CourseId_1}", form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.CourseId_1, result.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Title"), "title");
        form.Add(new StringContent("Description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("60"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await _client.PutAsync($"/api/v1/course/{NonExistentId}", form);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Title"), "title");
        form.Add(new StringContent("Description"), "description");
        form.Add(new StringContent(TestSeedIds.RefTerm_Technology.ToString()), "category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Programming.ToString()), "sub_category_id");
        form.Add(new StringContent(TestSeedIds.RefTerm_Easy.ToString()), "difficulty_level_id");
        form.Add(new StringContent("60"), "duration_minute");
        form.Add(new StringContent("false"), "is_drafted");

        var response = await unauthClient.PutAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}",
            form
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- DELETE ---

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenCourseExists()
    {
        var response = await _client.DeleteAsync($"/api/v1/course/{TestSeedIds.DeleteCourseId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.DeleteCourseId, result.Id);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/v1/course/{NonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.DeleteAsync($"/api/v1/course/{TestSeedIds.CourseId_1}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- ADMIN SUMMARY ---

    [Fact(Skip = "Known issue: AutoMapper mapping missing for AdminCourseSummaryDto")]
    public async Task GetAdminSummary_ShouldReturnOk_WhenAuthorized()
    {
        var response = await _client.GetAsync("/api/v1/course/admin-summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AdminCourseSummaryDto>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.True(result.TotalCourses > 0);
    }

    [Fact]
    public async Task GetAdminSummary_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/v1/course/admin-summary");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- GET FORUM MESSAGES ---

    [Fact]
    public async Task GetForumMessage_ShouldReturnOk_WhenCourseExists()
    {
        var response = await _client.GetAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/forum-message"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ForumMessageDto>>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetForumMessage_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/forum-message"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- CREATE FORUM MESSAGE ---

    [Fact]
    public async Task CreateForumMessage_ShouldReturnCreated_WhenValidRequest()
    {
        var dto = new CreateForumMessageRequestDto
        {
            MessageText = "This is a test forum message.",
        };

        var response = await _client.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/forum-message",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateForumMessage_ShouldReturnBadRequest_WhenMessageTextIsEmpty()
    {
        var dto = new CreateForumMessageRequestDto { MessageText = "" };

        var response = await _client.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/forum-message",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateForumMessage_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var dto = new CreateForumMessageRequestDto { MessageText = "Test message" };

        var response = await unauthClient.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/forum-message",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateForumMessage_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        var dto = new CreateForumMessageRequestDto
        {
            MessageText = "Message for non-existent course.",
        };

        var response = await _client.PostAsync(
            $"/api/v1/course/{NonExistentId}/forum-message",
            TestHelper.CreateJsonContent(dto)
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- ENROLL ---

    [Fact]
    public async Task Enroll_ShouldReturnCreated_WhenCourseExists()
    {
        var response = await _client.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_2}/enrollment",
            null
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Enroll_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        var response = await _client.PostAsync(
            $"/api/v1/course/{NonExistentId}/enrollment",
            null
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Enroll_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_2}/enrollment",
            null
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Enroll_ShouldReturnBadRequest_WhenAlreadyEnrolled()
    {
        var response = await _client.PostAsync(
            $"/api/v1/course/{TestSeedIds.CourseId_1}/enrollment",
            null
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- ACTIVE CATEGORY ---

    [Fact]
    public async Task GetActiveCategory_ShouldReturnOnlyCategoriesWithCourses()
    {
        var response = await _client.GetAsync("/api/v1/course/active-category");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<RefTermDto>>(
            TestHelper.JsonOptions
        );

        Assert.NotNull(result);

        Assert.Contains(result, c => c.Id == TestSeedIds.TechnologyCategoryId);
        Assert.Contains(result, c => c.Id == TestSeedIds.BusinessCategoryId);
        Assert.DoesNotContain(result, c => c.Name == "Programming");
        Assert.DoesNotContain(result, c => c.Name == "Management");
    }
}
