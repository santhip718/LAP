using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAP.Application.DTO.CourseContent;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LAP.Test.IntegrationTest.Controller;

public class CourseContentControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CourseContentControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCourseContentById_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Content_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, contentId, userId, roleName, "VIEW_COURSE_CONTENT");
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/course-content/{contentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseContentDetailDto>(TestHelper.JsonOptions);
        result.Should().NotBeNull();
        result!.Id.Should().Be(contentId);
    }

    [Fact]
    public async Task UpdateCompletionStatus_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Complete_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, contentId, userId, roleName, "UPDATE_COURSE_PROGRESS");
        await SeedEnrollmentAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        var request = new UpdateContentCompletionStatusRequest { IsCompleted = true };

        // Act
        var response = await _client.PutAsync(
            $"/api/V1/course-content/{contentId}/completion-status",
            TestHelper.CreateJsonContent(request)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result =
            await response.Content.ReadFromJsonAsync<UpdateContentCompletionStatusResponse>(TestHelper.SnakeCaseOptions);
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
    }

    private async Task SeedRequiredDataAsync(
        Guid courseId,
        Guid contentId,
        Guid userId,
        string roleName,
        string featureName
    )
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        // Seed Reference Data
        var rolesSet = new RefSet { Id = Guid.NewGuid(), Name = "Roles" };
        var categorySet = new RefSet { Id = Guid.NewGuid(), Name = "Category" };
        var difficultySet = new RefSet { Id = Guid.NewGuid(), Name = "Difficulty" };
        var contentTypeSet = new RefSet { Id = Guid.NewGuid(), Name = "ContentType" };
        var genderSet = new RefSet { Id = Guid.NewGuid(), Name = "Gender" };
        var designationSet = new RefSet { Id = Guid.NewGuid(), Name = "Designation" };

        var role = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            RefSetId = rolesSet.Id,
        };
        var category = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "IT",
            RefSetId = categorySet.Id,
        };
        var difficulty = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "Beginner",
            RefSetId = difficultySet.Id,
        };
        var contentType = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "Video",
            RefSetId = contentTypeSet.Id,
        };
        var gender = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "Male",
            RefSetId = genderSet.Id,
        };
        var designation = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "Engineer",
            RefSetId = designationSet.Id,
        };

        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = featureName,
            Method = "GET",
        };
        var mapping = new RoleFeatureMapping
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            FeatureId = feature.Id,
        };

        // Seed User and Person
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FullName = "Test User",
            Email = "test-" + userId + "@example.com",
            MobileNumber = "1234567890",
            DesignationId = designation.Id,
            GenderId = gender.Id,
        };
        var user = new User { Id = userId, PersonId = person.Id };
        var userRole = new UserRoleMapping
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
        };

        // Seed Course
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            CategoryId = category.Id,
            SubCategoryId = category.Id,
            DifficultyLevelId = difficulty.Id,
            CreatedByUserId = userId,
            DurationMinute = 60,
            IsDrafted = false,
        };

        // Seed Meta Topic
        var metaTopic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Name = "Test Topic",
            SequenceOrder = 1,
            DurationMinute = 30,
        };

        // Seed Course Content
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopicId = metaTopic.Id,
            Title = "Test Content",
            ContentTypeId = contentType.Id,
            SequenceOrder = 1,
            VideoUrl = "http://example.com/video.mp4",
        };

        dbContext.RefSet.AddRange(
            rolesSet,
            categorySet,
            difficultySet,
            contentTypeSet,
            genderSet,
            designationSet
        );
        dbContext.RefTerm.AddRange(role, category, difficulty, contentType, gender, designation);
        dbContext.Feature.Add(feature);
        dbContext.RoleFeatureMapping.Add(mapping);
        dbContext.Person.Add(person);
        dbContext.User.Add(user);
        dbContext.UserRoleMapping.Add(userRole);
        dbContext.Course.Add(course);
        dbContext.CourseMetaTopic.Add(metaTopic);
        dbContext.CourseContent.Add(content);

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedEnrollmentAsync(Guid courseId, Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            EnrolledOn = DateTime.UtcNow,
            EnrollmentStatus = true,
            ProgressPercentage = 0,
        };

        dbContext.Enrollment.Add(enrollment);
        await dbContext.SaveChangesAsync();
    }

    private void SetAuthHeaders(Guid userId, string role)
    {
        using var scope = _factory.Services.CreateScope();
        var jwtHelper =
            scope.ServiceProvider.GetRequiredService<LAP.Application.Interface.IHelper.IJwtHelper>();
        var tokenResponse = jwtHelper.GenerateToken(
            userId,
            "test@example.com",
            "Test User",
            new List<string> { role }
        );
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
    }
}
