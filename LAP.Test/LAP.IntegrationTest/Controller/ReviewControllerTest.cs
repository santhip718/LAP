using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.Review;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LAP.Test.IntegrationTest.Controller;

public class ReviewControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReviewControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateReview_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_Review_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, userId, roleName, "CREATE_REVIEW");
        await SeedEnrollmentAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        var request = new CreateReviewRequestDto { Rating = 5, ReviewText = "Great course!" };

        // Act
        var response = await _client.PostAsync(
            $"/api/V1/review/course/{courseId}",
            TestHelper.CreateJsonContent(request)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ReviewDto>(TestHelper.SnakeCaseOptions);
        result.Should().NotBeNull();
        result!.Rating.Should().Be(5);
        result.ReviewText.Should().Be("Great course!");
    }

    [Fact]
    public async Task GetCourseReviews_WithValidCourseId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_GetReviews_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, userId, roleName, "VIEW_REVIEW");
        await SeedReviewAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/review/course/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedReviewsDto>(TestHelper.JsonOptions);
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateReview_WithOwner_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_UpdateReview_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, userId, roleName, "MANAGE_REVIEW");
        var reviewId = await SeedReviewAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        var request = new UpdateReviewRequestDto { Rating = 4, ReviewText = "Updated review" };

        // Act
        var response = await _client.PutJsonAsync($"/api/V1/review/{reviewId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ReviewDto>(TestHelper.SnakeCaseOptions);
        result.Should().NotBeNull();
        result!.Rating.Should().Be(4);
        result.ReviewText.Should().Be("Updated review");
    }

    [Fact]
    public async Task DeleteReview_WithOwner_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleName = "Student_DeleteReview_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(courseId, userId, roleName, "MANAGE_REVIEW");
        var reviewId = await SeedReviewAsync(courseId, userId);
        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.DeleteAsync($"/api/V1/review/{reviewId}");

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HTTP {(int)response.StatusCode} Error: {error}");
        }
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(TestHelper.JsonOptions);
        result.Should().NotBeNull();
    }

    private async Task SeedRequiredDataAsync(
        Guid courseId,
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
            Method = "POST",
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

        dbContext.RefSet.AddRange(rolesSet, categorySet, difficultySet, genderSet, designationSet);
        dbContext.RefTerm.AddRange(role, category, difficulty, gender, designation);
        dbContext.Feature.Add(feature);
        dbContext.RoleFeatureMapping.Add(mapping);
        dbContext.Person.Add(person);
        dbContext.User.Add(user);
        dbContext.UserSecret.Add(secret);
        dbContext.UserRoleMapping.Add(userRole);
        dbContext.Course.Add(course);

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

    private async Task<Guid> SeedReviewAsync(Guid courseId, Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        var review = new Review
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            Rating = 5,
            ReviewText = "Seed review",
            DateCreated = DateTime.UtcNow,
        };

        dbContext.Review.Add(review);
        await dbContext.SaveChangesAsync();
        return review.Id;
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
