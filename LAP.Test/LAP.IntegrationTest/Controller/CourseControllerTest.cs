using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.Paginated;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LAP.Test.IntegrationTest.Controller;

public class CourseControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CourseControllerTest(CustomWebApplicationFactory factory)
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
        var roleRefSet = db.RefSet.FirstOrDefault(x => x.Name == "Role");
        var adminRole =
            roleRefSet != null
                ? db.RefTerm.FirstOrDefault(x => x.RefSetId == roleRefSet.Id && x.Name == "Admin")
                : null;

        if (adminRole == null)
            return;

        var person = new Person
        {
            Id = Guid.NewGuid(),
            FullName = "Course Test Admin",
            Email = $"courseadmin{Guid.NewGuid():N}@example.com",
            MobileNumber = "1234567890",
            DesignationId = designation?.Id ?? Guid.Empty,
            GenderId = gender?.Id ?? Guid.Empty,
        };
        var user = new User
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            OverallScore = 0,
        };
        var userRole = new UserRoleMapping
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = adminRole.Id,
        };

        db.Person.Add(person);
        db.User.Add(user);
        db.UserRoleMapping.Add(userRole);
        await db.SaveChangesAsync();

        using var scope = _factory.Services.CreateScope();
        var jwtHelper =
            scope.ServiceProvider.GetRequiredService<LAP.Application.Interface.IHelper.IJwtHelper>();
        var tokenResponse = jwtHelper.GenerateToken(
            user.Id,
            person.Email,
            person.FullName,
            new List<string> { "Admin" }
        );

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.AccessToken
        );
    }

    [Fact]
    public async Task GetCourseAssessmentHistory_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_AssHist_" + Guid.NewGuid().ToString("N");
        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_ASSESSMENT_HISTORY", true);
        await SeedAssessmentHistoryAsync(assessmentId, courseId, userId);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/course/{courseId}/assessment-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedAssessmentHistoryDto>(
            TestHelper.JsonOptions
        );
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCourses_WithValidRequest_Returns200OkWithPaginatedData()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_GetCourses_" + Guid.NewGuid().ToString("N");
        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_COURSE", true);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync("/api/V1/course?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedCoursesDto>(
            TestHelper.JsonOptions
        );
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetRecommendations_WithValidUser_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Recs_" + Guid.NewGuid().ToString("N");
        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_RECOMMENDATION", true);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync("/api/V1/course/recommendation");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CourseSummaryDto>>(
            TestHelper.JsonOptions
        );
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCourseOverview_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Overview_" + Guid.NewGuid().ToString("N");
        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_COURSE", true);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/course/{courseId}/overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseOverviewDto>(
            TestHelper.SnakeCaseOptions
        );
        result.Should().NotBeNull();
        result!.Id.Should().Be(courseId);
    }


    [Fact]
    public async Task GetProgress_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Progress_" + Guid.NewGuid().ToString("N");
        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_COURSE_PROGRESS", true);
        await SeedEnrollmentAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/course/{courseId}/progress");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseProgressResponseDto>(
            TestHelper.SnakeCaseOptions
        );
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().NotBeEmpty();
    }

    private async Task SeedRequiredDataAsync(
        Guid courseId,
        Guid userId,
        string roleName,
        string featureName,
        bool mapFeatureToRole
    )
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        // Seed Reference Data
        var rolesSet = new RefSet { Id = Guid.NewGuid(), Name = "Roles" };
        var genderSet = new RefSet { Id = Guid.NewGuid(), Name = "Gender" };
        var designationSet = new RefSet { Id = Guid.NewGuid(), Name = "Designation" };
        var categorySet = new RefSet { Id = Guid.NewGuid(), Name = "Category" };
        var difficultySet = new RefSet { Id = Guid.NewGuid(), Name = "Difficulty" };

        var role = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            RefSetId = rolesSet.Id,
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
            Name = "Software Engineer",
            RefSetId = designationSet.Id,
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

        // Ensure the feature exists (for DynamicPolicyProvider)
        var feature = dbContext.Feature.FirstOrDefault(f => f.Name == featureName);
        if (feature == null)
        {
            feature = new Feature
            {
                Id = Guid.NewGuid(),
                Name = featureName,
                Method = "POST",
            };
            dbContext.Feature.Add(feature);
        }

        if (mapFeatureToRole)
        {
            var mapping = new RoleFeatureMapping
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                FeatureId = feature.Id,
            };
            dbContext.RoleFeatureMapping.Add(mapping);
        }

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

        var user = new User
        {
            Id = userId,
            PersonId = person.Id,
            OverallScore = 0,
        };
        var userRole = new UserRoleMapping
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
        };
        var secret = new UserSecret
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PasswordHash = "testHash",
            PasswordSalt = "testSalt",
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

        dbContext.RefSet.AddRange(rolesSet, genderSet, designationSet, categorySet, difficultySet);
        dbContext.RefTerm.AddRange(role, gender, designation, category, difficulty);
        dbContext.Person.Add(person);
        dbContext.User.Add(user);
        dbContext.UserSecret.Add(secret);
        dbContext.UserRoleMapping.Add(userRole);
        dbContext.Course.Add(course);

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetLeaderboardByCourseId_ShouldReturnOk_WhenCourseExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        Guid courseId = db.Course.First(x => x.Title == "Introduction to Programming").Id;

        HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/course/{courseId}/leaderboard"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<LeaderboardDto>? result = await response.Content.ReadFromJsonAsync<
            List<LeaderboardDto>
        >(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLeaderboardByCourseId_ShouldReturnBadRequest_WhenCourseIdEmpty()
    {
        HttpResponseMessage response = await _client.GetAsync(
            "/api/v1/course/00000000-0000-0000-0000-000000000000/leaderboard"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLeaderboardByCourseId_ShouldReturnUnauthorized_WhenNoToken()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        Guid courseId = db.Course.First(x => x.Title == "Introduction to Programming").Id;

        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync(
            $"/api/v1/course/{courseId}/leaderboard"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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

    private async Task SeedAssessmentHistoryAsync(Guid assessmentId, Guid courseId, Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        var assessment = new Assessment
        {
            Id = assessmentId,
            CourseId = courseId,
            Title = "Test Assessment",
            TotalMark = 100,
            PassingMark = 50,
            DurationMinute = 30,
        };

        var history = new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            AssessmentId = assessmentId,
            UserId = userId,
            StartedOn = DateTime.UtcNow.AddMinutes(-30),
            CompletedOn = DateTime.UtcNow.AddMinutes(-10),
            Score = 80,
            WeightedScore = 80,
            //  Passed = true,
        };

        dbContext.Assessment.Add(assessment);
        dbContext.AssessmentHistory.Add(history);
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
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                tokenResponse.AccessToken
            );
    }
}
