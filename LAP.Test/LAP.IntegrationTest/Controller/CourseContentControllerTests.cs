using System.Net;
using System.Net.Http.Json;
using LAP.Application.DTO.Common;
using LAP.IntegrationTest;

namespace LAP.Test.IntegrationTest.Controller;

public class CourseContentControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string SeedUserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    private const string SeedUserEmail = "sathish@example.com";
    private static readonly Guid NonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public CourseContentControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.SeedDatabase();

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("TestUserId", SeedUserId);
        _client.DefaultRequestHeaders.Add("TestUserEmail", SeedUserEmail);
        _client.DefaultRequestHeaders.Add("TestRole", "Admin");
    }

    // --- CREATE ---

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValidRequest()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("New Meta Topic"), "meta_topic");
        form.Add(new StringContent("1"), "meta_topic_order");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Lesson 1"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");
        form.Add(new StringContent("https://example.com/video.mp4"), "video_url");
        form.Add(new StringContent("1"), "sequence_order");

        var response = await _client.PostAsync("/api/v1/course-content", form);

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
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Meta Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent(""), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");

        var response = await _client.PostAsync("/api/v1/course-content", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Meta Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Title"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");

        var response = await unauthClient.PostAsync("/api/v1/course-content", form);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WithoutMetaTopicOrder()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Auto Order Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Auto Ordered Lesson"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");
        form.Add(new StringContent("https://example.com/auto.mp4"), "video_url");
        form.Add(new StringContent("99"), "sequence_order");

        var response = await _client.PostAsync("/api/v1/course-content", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WithoutSequenceOrder()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Auto Sequence Topic"), "meta_topic");
        form.Add(new StringContent("1"), "meta_topic_order");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Auto Sequenced Lesson"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");
        form.Add(new StringContent("https://example.com/auto-seq.mp4"), "video_url");

        var response = await _client.PostAsync("/api/v1/course-content", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WithExistingMetaTopic()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Getting Started"), "meta_topic");
        form.Add(new StringContent("2"), "meta_topic_order");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Another Lesson"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Pdf.ToString()), "content_type_id");
        form.Add(new StringContent("1"), "sequence_order");

        var response = await _client.PostAsync("/api/v1/course-content", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    // --- UPDATE ---

    [Fact]
    public async Task Update_ShouldReturnOk_WhenCourseContentExists()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Updated Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Updated Lesson Title"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Pdf.ToString()), "content_type_id");
        form.Add(new StringContent("1"), "sequence_order");

        var response = await _client.PutAsync(
            $"/api/v1/course-content/{TestSeedIds.CourseContentId_1}",
            form
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.CourseContentId_1, result.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCourseContentDoesNotExist()
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Title"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");

        var response = await _client.PutAsync($"/api/v1/course-content/{NonExistentId}", form);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(TestSeedIds.CourseId_1.ToString()), "course_id");
        form.Add(new StringContent("Topic"), "meta_topic");
        form.Add(new StringContent("30"), "meta_duration_minute");
        form.Add(new StringContent("Title"), "title");
        form.Add(new StringContent(TestSeedIds.RefTerm_Video.ToString()), "content_type_id");

        var response = await unauthClient.PutAsync(
            $"/api/v1/course-content/{TestSeedIds.CourseContentId_1}",
            form
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- DELETE ---

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenCourseContentExists()
    {
        var response = await _client.DeleteAsync(
            $"/api/v1/course-content/{TestSeedIds.DeleteCourseContentId}"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.JsonOptions
        );
        Assert.NotNull(result);
        Assert.Equal(TestSeedIds.DeleteCourseContentId, result.Id);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCourseContentDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/v1/course-content/{NonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNoAuthHeaders()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.DeleteAsync(
            $"/api/v1/course-content/{TestSeedIds.CourseContentId_1}"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
