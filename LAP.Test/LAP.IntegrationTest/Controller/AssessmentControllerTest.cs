using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Test.IntegrationTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LAP.Test.IntegrationTest.Controller;

public class AssessmentControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AssessmentControllerTest(CustomWebApplicationFactory factory)
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
            FullName = "Assessment Test Admin",
            Email = $"assessadmin{Guid.NewGuid():N}@example.com",
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
    public async Task SubmitAssessment_WithValidAnswers_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var roleName = "Student_Submit_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(
            courseId,
            assessmentId,
            userId,
            questionId,
            roleName,
            "SUBMIT_ASSESSMENT"
        );
        await SeedEnrollmentAsync(courseId, userId);

        SetAuthHeaders(userId, roleName);

        var request = new AssessmentSubmitRequestDto
        {
            UserId = userId,
            StartedOn = DateTime.UtcNow.AddMinutes(-10),
            Answer = new List<Answer>
            {
                new Answer { QuestionId = questionId, SelectedAnswer = "A" },
            },
        };

        // Act
        var response = await _client.PostAsync(
            $"/api/V1/assessment/{assessmentId}/submit",
            TestHelper.CreateJsonContent(request)
        );

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HTTP {(int)response.StatusCode} Error: {error}");
        }
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SubmitAssessmentResponseDto>(
            TestHelper.SnakeCaseOptions
        );
        result.Should().NotBeNull();
        result!.AssessmentId.Should().Be(assessmentId);
        result.Passed.Should().BeTrue();
        result.Status.Should().Be("Completed");
        result.WeakTopic.Should().NotBeNull();
        result.WeakTopic.Should().HaveCount(1);
        result.WeakTopic.First().TopicName.Should().Be("Test Topic");
        result.Answers.Should().NotBeNull();
        result.Answers.Should().HaveCount(1);
        result.Answers.First().QuestionId.Should().Be(questionId);
        result.Answers.First().IsCorrect.Should().BeTrue();
    }

    [Fact]
    public async Task GetAssessmentResult_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var roleName = "Student_Result_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(
            courseId,
            assessmentId,
            userId,
            questionId,
            roleName,
            "VIEW_ASSESSMENT"
        );
        await SeedEnrollmentAsync(courseId, userId);
        // Seed multiple history records for the user
        await SeedAssessmentHistoryAsync(assessmentId, courseId, userId, score: 80, daysAgo: 10);
        await SeedAssessmentHistoryAsync(assessmentId, courseId, userId, score: 90, daysAgo: 5);
        await SeedAssessmentHistoryAsync(assessmentId, courseId, userId, score: 70, daysAgo: 1);

        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync($"/api/V1/assessment/{assessmentId}/result");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AssessmentResultResponseDto>(
            TestHelper.SnakeCaseOptions
        );
        result.Should().NotBeNull();
        result!.AssessmentId.Should().Be(assessmentId);
        result.Attempts.Should().HaveCount(3);
        result.Attempts[0].AttemptNumber.Should().Be(1);
        result.Attempts[0].Score.Should().Be(80);
        result.Attempts[1].AttemptNumber.Should().Be(2);
        result.Attempts[1].Score.Should().Be(90);
        result.Attempts[2].AttemptNumber.Should().Be(3);
        result.Attempts[2].Score.Should().Be(70);
    }

    [Fact]
    public async Task GetUserAssessmentHistory_WithValidId_Returns200Ok()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var roleName = "Student_UserHist_" + Guid.NewGuid().ToString("N");

        await SeedRequiredDataAsync(
            courseId,
            assessmentId,
            userId,
            questionId,
            roleName,
            "VIEW_ASSESSMENT_HISTORY"
        );
        await SeedEnrollmentAsync(courseId, userId);
        await SeedAssessmentHistoryAsync(assessmentId, courseId, userId);

        SetAuthHeaders(userId, roleName);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/assessment/user/{userId}/assessment-history"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result =
            await response.Content.ReadFromJsonAsync<PaginatedAssessmentHistoryResponseDto>(
                TestHelper.SnakeCaseOptions
            );
        result.Should().NotBeNull();
        result!.Item.Should().NotBeEmpty();
    }

    private async Task SeedRequiredDataAsync(
        Guid courseId,
        Guid assessmentId,
        Guid userId,
        Guid questionId,
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
        var questionTypeSet = new RefSet { Id = Guid.NewGuid(), Name = "QuestionType" };
        var genderSet = new RefSet { Id = Guid.NewGuid(), Name = "Gender" };
        var designationSet = new RefSet { Id = Guid.NewGuid(), Name = "Designation" };
        var existingTierSet = dbContext.RefSet.FirstOrDefault(rs => rs.Name == "Tier");
        var tierSet = existingTierSet ?? new RefSet { Id = Guid.NewGuid(), Name = "Tier" };

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
        var questionType = new RefTerm
        {
            Id = Guid.NewGuid(),
            Name = "MCQ",
            RefSetId = questionTypeSet.Id,
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

        // Seed all tiers to be safe, using existing Tier RefSet if already seeded
        var existingTierNames = dbContext
            .RefTerm.Where(rt => rt.RefSetId == tierSet.Id)
            .Select(rt => rt.Name)
            .ToHashSet();
        var tierNames = new[]
        {
            "Code Cadet",
            "Syntax Voyager",
            "Logic Architect",
            "Runtime Titan",
            "System Sovereign",
        };
        var tierTerms = tierNames
            .Where(name => !existingTierNames.Contains(name))
            .Select(name => new RefTerm
            {
                Id = Guid.NewGuid(),
                Name = name,
                RefSetId = tierSet.Id,
            })
            .ToList();

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

        // Seed Meta Topic
        var metaTopic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Name = "Test Topic",
            SequenceOrder = 1,
            DurationMinute = 30,
        };

        // Seed Assessment
        var assessment = new Assessment
        {
            Id = assessmentId,
            CourseId = courseId,
            Title = "Test Assessment",
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 20,
        };

        // Seed Question
        var question = new Question
        {
            Id = questionId,
            AssessmentId = assessmentId,
            MetaTopicId = metaTopic.Id,
            QuestionTypeId = questionType.Id,
            QuestionText = "What is 1+1?",
            OptionList = new List<string> { "1", "2", "3" },
            Answer = "A",
            Weight = 10,
        };

        var refSets = new List<RefSet>
        {
            rolesSet,
            categorySet,
            difficultySet,
            questionTypeSet,
            genderSet,
            designationSet,
        };
        if (existingTierSet == null)
            refSets.Add(tierSet);
        dbContext.RefSet.AddRange(refSets.ToArray());
        dbContext.RefTerm.AddRange(
            new[] { role, category, difficulty, questionType, gender, designation }
                .Concat(tierTerms)
                .ToArray()
        );
        dbContext.Feature.Add(feature);
        dbContext.RoleFeatureMapping.Add(mapping);
        dbContext.Person.Add(person);
        dbContext.User.Add(user);
        dbContext.UserSecret.Add(secret);
        dbContext.UserRoleMapping.Add(userRole);
        dbContext.Course.Add(course);
        dbContext.CourseMetaTopic.Add(metaTopic);
        dbContext.Assessment.Add(assessment);
        dbContext.Question.Add(question);

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAssessments_ShouldReturnOkAndList()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/assessment");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        PaginatedAssessmentsDto? result = await response.Content.ReadFromJsonAsync<
            PaginatedAssessmentsDto
        >(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAllAssessments_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync("/api/v1/assessment");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAssessmentOverviewByCourseId_ShouldReturnOk_WhenCourseExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var course = CreateTempCourseForTest(db, "Assessment Overview Course");
        db.Assessment.Add(
            new Assessment
            {
                CourseId = course.Id,
                Title = "Assessment Overview Test",
                TotalMark = 20,
                PassingMark = 10,
                DurationMinute = 30,
            }
        );
        db.SaveChanges();

        HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/course/{course.Id}/assessment/overview"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<AssessmentOverviewDto>? result = await response.Content.ReadFromJsonAsync<
            List<AssessmentOverviewDto>
        >(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAssessmentOverviewByCourseId_ShouldReturnEmpty_WhenCourseHasNoAssessment()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/course/{Guid.NewGuid()}/assessment/overview"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<AssessmentOverviewDto>? result = await response.Content.ReadFromJsonAsync<
            List<AssessmentOverviewDto>
        >(TestHelper.SnakeCaseOptions);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetQuestionsByAssessmentId_ShouldReturnOk_WhenAssessmentExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var assessment = CreateTempAssessmentForTest(db, "Questions Assessment");
        var metaTopic = db.CourseMetaTopic.First(x => x.CourseId == assessment.CourseId);
        var questionType = db.RefTerm.First(x => x.Name == "MCQ");
        db.Question.Add(
            new Question
            {
                AssessmentId = assessment.Id,
                MetaTopicId = metaTopic.Id,
                QuestionTypeId = questionType.Id,
                QuestionText = "What is integration testing?",
                OptionList = new List<string> { "A", "B" },
                Answer = "A",
                Weight = 1,
            }
        );
        db.SaveChanges();

        HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/assessment/{assessment.Id}/question"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<QuestionDto>? result = await response.Content.ReadFromJsonAsync<List<QuestionDto>>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetQuestionsByAssessmentId_ShouldReturnNotFound_WhenAssessmentDoesNotExist()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}/question"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAssessment_ShouldReturnOk_WhenAssessmentExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var course = CreateTempCourseForTest(db, "Update Assessment Course");
        var assessment = new LAP.Domain.Entity.Assessment
        {
            CourseId = course.Id,
            Title = "Temp Update Assessment",
            TotalMark = 50,
            PassingMark = 25,
            DurationMinute = 30,
        };
        db.Assessment.Add(assessment);
        db.SaveChanges();
        Guid assessmentId = assessment.Id;

        var dto = new UpdateAssessmentRequestDto
        {
            Title = "Updated Temp Assessment",
            Description = "Updated self-contained test data",
            TotalMark = 100,
            PassingMark = 50,
            DurationMinute = 90,
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/{assessmentId}",
            dto
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        SuccessResponse? result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task UpdateAssessment_ShouldReturnNotFound_WhenAssessmentDoesNotExist()
    {
        var dto = new UpdateAssessmentRequestDto
        {
            Title = "Ghost Assessment",
            Description = "Does not exist",
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 30,
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAssessment_ShouldReturnOk_WhenAssessmentExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var course = CreateTempCourseForTest(db, "Delete Assessment Course");
        var assessment = new LAP.Domain.Entity.Assessment
        {
            CourseId = course.Id,
            Title = "Temp Delete Assessment",
            TotalMark = 50,
            PassingMark = 25,
            DurationMinute = 30,
        };
        db.Assessment.Add(assessment);
        db.SaveChanges();
        Guid assessmentId = assessment.Id;

        HttpResponseMessage response = await _client.DeleteAsync(
            $"/api/v1/assessment/{assessmentId}"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        SuccessResponse? result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAssessment_ShouldReturnNotFound_WhenAssessmentDoesNotExist()
    {
        HttpResponseMessage response = await _client.DeleteAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_ShouldReturnOk_WhenQuestionExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var assessment = CreateTempAssessmentForTest(db, "Update Question Assessment");
        var metaTopic = db.CourseMetaTopic.First(x => x.CourseId == assessment.CourseId);
        var questionType = db.RefTerm.First(x => x.Name == "MCQ");
        var question = new LAP.Domain.Entity.Question
        {
            AssessmentId = assessment.Id,
            MetaTopicId = metaTopic.Id,
            QuestionTypeId = questionType.Id,
            QuestionText = "Temp question for update?",
            OptionList = new List<string> { "A", "B", "C" },
            Answer = "A",
            Weight = 1,
        };
        db.Question.Add(question);
        db.SaveChanges();
        Guid questionId = question.Id;

        var dto = new UpdateQuestionRequestDto
        {
            QuestionText = "Updated temp question?",
            Weight = 3,
            OptionList = new List<string> { "X", "Y", "Z" },
            Answer = "X",
            QuestionTypeId = questionType.Id,
            MetaTopicId = metaTopic.Id.ToString(),
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/question/{questionId}",
            dto
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        SuccessResponse? result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task UpdateQuestion_ShouldReturnNotFound_WhenQuestionDoesNotExist()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var questionType = db.RefTerm.First(x => x.Name == "MCQ");

        var dto = new UpdateQuestionRequestDto
        {
            QuestionText = "Ghost Question?",
            Weight = 1,
            OptionList = new List<string> { "A", "B" },
            Answer = "A",
            QuestionTypeId = questionType.Id,
            MetaTopicId = Guid.NewGuid().ToString(),
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/question/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_ShouldReturnOk_WhenQuestionExists()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var assessment = CreateTempAssessmentForTest(db, "Delete Question Assessment");
        var metaTopic = db.CourseMetaTopic.First(x => x.CourseId == assessment.CourseId);
        var questionType = db.RefTerm.First(x => x.Name == "MCQ");
        var question = new LAP.Domain.Entity.Question
        {
            AssessmentId = assessment.Id,
            MetaTopicId = metaTopic.Id,
            QuestionTypeId = questionType.Id,
            QuestionText = "Temp question to delete?",
            OptionList = new List<string> { "Yes", "No" },
            Answer = "Yes",
            Weight = 1,
        };
        db.Question.Add(question);
        db.SaveChanges();
        Guid questionId = question.Id;

        HttpResponseMessage response = await _client.DeleteAsync(
            $"/api/v1/assessment/question/{questionId}"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        SuccessResponse? result = await response.Content.ReadFromJsonAsync<SuccessResponse>(
            TestHelper.SnakeCaseOptions
        );
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteQuestion_ShouldReturnNotFound_WhenQuestionDoesNotExist()
    {
        HttpResponseMessage response = await _client.DeleteAsync(
            $"/api/v1/assessment/question/{Guid.NewGuid()}"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(Guid.NewGuid().ToString()), "CourseId");
        content.Add(new StringContent("Unauthorized Test"), "Title");
        content.Add(new StringContent("10"), "PassingMark");
        content.Add(new StringContent("60"), "DurationMinute");

        HttpResponseMessage response = await unauthClient.PostAsync("/api/v1/assessment", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ExportTemplate_ShouldReturnOk_WhenTemplateFileExists()
    {
        string templateDir = _factory.QuestionTemplatePath;
        if (!Directory.Exists(templateDir))
        {
            Directory.CreateDirectory(templateDir);
        }
        string templateFilePath = Path.Combine(
            templateDir,
            CommonConstants.QuestionTemplateFileName
        );
        if (!File.Exists(templateFilePath))
        {
            await File.WriteAllBytesAsync(templateFilePath, [0x00]);
        }

        HttpResponseMessage response = await _client.GetAsync("/api/v1/assessment/export-template");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            response.Content.Headers.ContentType?.MediaType
        );
        Assert.NotNull(response.Content.Headers.ContentDisposition);
        Assert.Contains(
            "Question Template File.xlsx",
            response.Content.Headers.ContentDisposition?.FileName ?? ""
        );
    }

    // ─── Validation (400) Tests ─────────────────────────────────────

    [Fact]
    public async Task UpdateAssessment_ShouldReturnBadRequest_WhenTitleEmpty()
    {
        var dto = new UpdateAssessmentRequestDto
        {
            Title = "",
            Description = "Desc",
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 30,
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_ShouldReturnBadRequest_WhenQuestionTextEmpty()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        var questionType = db.RefTerm.First(x => x.Name == "MCQ");
        var metaTopic = db.CourseMetaTopic.First();

        var dto = new UpdateQuestionRequestDto
        {
            QuestionText = "",
            OptionList = new List<string> { "A", "B" },
            Answer = "A",
            Weight = 1,
            QuestionTypeId = questionType.Id,
            MetaTopicId = metaTopic.Id.ToString(),
        };

        HttpResponseMessage response = await _client.PutJsonAsync(
            $"/api/v1/assessment/question/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionsByAssessmentId_ShouldReturnBadRequest_WhenIdEmpty()
    {
        HttpResponseMessage response = await _client.GetAsync(
            "/api/v1/assessment/00000000-0000-0000-0000-000000000000/question"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAssessment_ShouldReturnBadRequest_WhenIdEmpty()
    {
        HttpResponseMessage response = await _client.DeleteAsync(
            "/api/v1/assessment/00000000-0000-0000-0000-000000000000"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_ShouldReturnBadRequest_WhenValidationFails()
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(Guid.Empty.ToString()), "CourseId");
        content.Add(new StringContent(""), "Title");
        content.Add(new StringContent("0"), "PassingMark");
        content.Add(new StringContent("0"), "DurationMinute");

        HttpResponseMessage response = await _client.PostAsync("/api/v1/assessment", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─── Unauthorized (401) Tests ────────────────────────────────────

    [Fact]
    public async Task GetAssessmentOverviewByCourseId_ShouldReturnUnauthorized_WhenNoToken()
    {
        using LearningAssessmentDbContext db = _factory.CreateDbContext();
        Guid courseId = db.Course.First(x => x.Title == "Introduction to Programming").Id;

        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync(
            $"/api/v1/course/{courseId}/assessment/overview"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAssessment_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        var dto = new UpdateAssessmentRequestDto
        {
            Title = "Unauthorized",
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 30,
        };

        HttpResponseMessage response = await unauthClient.PutJsonAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAssessment_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.DeleteAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        var dto = new UpdateQuestionRequestDto
        {
            QuestionText = "Unauthorized?",
            OptionList = new List<string> { "A", "B" },
            Answer = "A",
            Weight = 1,
            QuestionTypeId = Guid.NewGuid(),
            MetaTopicId = Guid.NewGuid().ToString(),
        };

        HttpResponseMessage response = await unauthClient.PutJsonAsync(
            $"/api/v1/assessment/question/{Guid.NewGuid()}",
            dto
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.DeleteAsync(
            $"/api/v1/assessment/question/{Guid.NewGuid()}"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionsByAssessmentId_ShouldReturnUnauthorized_WhenNoToken()
    {
        var unauthClient = _factory.CreateClient();
        HttpResponseMessage response = await unauthClient.GetAsync(
            $"/api/v1/assessment/{Guid.NewGuid()}/question"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private Course CreateTempCourseForTest(LearningAssessmentDbContext db, string title)
    {
        var category = db.RefTerm.First(x => x.Name == "Technology");
        var difficulty = db.RefTerm.First(x => x.Name == "Beginner");
        var user = db.User.First();

        var course = new Course
        {
            Title = title,
            CategoryId = category.Id,
            SubCategoryId = category.Id,
            DifficultyLevelId = difficulty.Id,
            CreatedByUserId = user.Id,
        };
        db.Course.Add(course);
        db.SaveChanges();
        return course;
    }

    private Assessment CreateTempAssessmentForTest(
        LearningAssessmentDbContext db,
        string title
    )
    {
        var course = CreateTempCourseForTest(db, $"{title} Course");
        var metaTopic = new CourseMetaTopic
        {
            CourseId = course.Id,
            Name = $"{title} Topic",
            SequenceOrder = 1,
            DurationMinute = 10,
        };
        var assessment = new Assessment
        {
            CourseId = course.Id,
            Title = title,
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 20,
        };

        db.CourseMetaTopic.Add(metaTopic);
        db.Assessment.Add(assessment);
        db.SaveChanges();
        return assessment;
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

    private async Task SeedAssessmentHistoryAsync(
        Guid assessmentId,
        Guid courseId,
        Guid userId,
        decimal score = 80,
        int daysAgo = 1
    )
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        var assessment = await dbContext.Assessment.FindAsync(assessmentId);
        if (assessment == null)
        {
            assessment = new Assessment
            {
                Id = assessmentId,
                CourseId = courseId,
                Title = "Test Assessment",
                TotalMark = 100,
                PassingMark = 50,
                DurationMinute = 30,
            };
            dbContext.Assessment.Add(assessment);
        }

        // We also need a tier for the history record if it's required by constraints
        var tierSet = await dbContext.RefSet.FirstOrDefaultAsync(rs => rs.Name == "Tier");
        if (tierSet == null)
        {
            tierSet = new RefSet { Id = Guid.NewGuid(), Name = "Tier" };
            dbContext.RefSet.Add(tierSet);
        }
        var tier = await dbContext.RefTerm.FirstOrDefaultAsync(rt =>
            rt.RefSetId == tierSet.Id && rt.Name == "Runtime Titan"
        );
        if (tier == null)
        {
            tier = new RefTerm
            {
                Id = Guid.NewGuid(),
                Name = "Runtime Titan",
                RefSetId = tierSet.Id,
            };
            dbContext.RefTerm.Add(tier);
        }

        var now = DateTime.UtcNow;
        var history = new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            AssessmentId = assessmentId,
            UserId = userId,
            StartedOn = now.AddDays(-daysAgo).AddMinutes(-30),
            CompletedOn = now.AddDays(-daysAgo).AddMinutes(-10),
            Score = score,
            WeightedScore = score,
            TierAwardedId = tier.Id,
        };

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
