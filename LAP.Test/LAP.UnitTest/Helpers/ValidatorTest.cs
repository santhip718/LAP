using FluentValidation;
using FluentValidation.TestHelper;
using LAP.Application.DTO.Assessment;
using Moq;
using LAP.Application.Interface.IService;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.CourseContent;
using LAP.Application.DTO.CourseReview;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Feature.Auth.Command;
using LAP.Application.Feature.Course.Command;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Feature.CourseContent.Query;
using LAP.Application.Feature.CourseReview.Command;
using LAP.Application.Feature.CourseReview.Query;
using LAP.Application.Feature.ReferenceData.Query;
using LAP.Application.Interface.IService;
using Moq;

namespace LAP.UnitTest.Helpers;

public class ValidatorTest
{
    [Fact]
    public void GetUserAssessmentHistoryValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetUserAssessmentHistoryValidator();
        var query = new GetUserAssessmentHistoryQuery(Guid.NewGuid(), 1, 10);

        var result = validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetUserAssessmentHistoryValidator_ShouldFail_WhenUserIdEmpty()
    {
        var validator = new GetUserAssessmentHistoryValidator();
        var query = new GetUserAssessmentHistoryQuery(Guid.Empty, 1, 10);

        var result = validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void GetAssessmentResultValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetAssessmentResultValidator();
        var query = new GetAssessmentResultQuery(Guid.NewGuid());

        var result = validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetAssessmentResultValidator_ShouldFail_WhenAssessmentIdEmpty()
    {
        var validator = new GetAssessmentResultValidator();

        var result = validator.TestValidate(new GetAssessmentResultQuery(Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.AssessmentId);
    }

    [Fact]
    public void LoginValidator_ShouldPass_WithValidInput()
    {
        var validator = new LoginValidator();
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "password123" };

        var result = validator.TestValidate(new LoginCommand(dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LoginValidator_ShouldFail_WhenEmailEmpty()
    {
        var validator = new LoginValidator();
        var dto = new LoginRequestDto { Email = "", Password = "password123" };

        var result = validator.TestValidate(new LoginCommand(dto));

        result.ShouldHaveValidationErrorFor(x => x.Dto.Email);
    }

    [Fact]
    public void LoginValidator_ShouldFail_WhenPasswordEmpty()
    {
        var validator = new LoginValidator();
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "" };

        var result = validator.TestValidate(new LoginCommand(dto));

        result.ShouldHaveValidationErrorFor(x => x.Dto.Password);
    }

    [Fact]
    public async Task RegisterValidator_ShouldPass_WithValidInput()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var validator = new RegisterValidator(authServiceMock.Object);
        var dto = new RegisterRequestDto
        {
            FullName = "John Doe",
            Email = "john@test.com",
            MobileNumber = "1234567890",
            Password = "Password@123",
        };

        var result = await validator.TestValidateAsync(new RegisterCommand(dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task RegisterValidator_ShouldFail_WhenPasswordTooShort()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var validator = new RegisterValidator(authServiceMock.Object);
        var dto = new RegisterRequestDto
        {
            FullName = "John Doe",
            Email = "john@test.com",
            MobileNumber = "1234567890",
            Password = "short",
        };

        var result = await validator.TestValidateAsync(new RegisterCommand(dto));

        result.ShouldHaveValidationErrorFor(x => x.Dto.Password);
    }

    [Fact]
    public void LogoutValidator_ShouldPass_WithValidInput()
    {
        var validator = new LogoutValidator();

        var result = validator.TestValidate(new LogoutCommand("valid-token"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LogoutValidator_ShouldFail_WhenTokenEmpty()
    {
        var validator = new LogoutValidator();

        var result = validator.TestValidate(new LogoutCommand(""));

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void RefreshTokenValidator_ShouldPass_WithValidInput()
    {
        var validator = new RefreshTokenValidator();
        var dto = new RefreshRequestDto { RefreshToken = "valid-token" };

        var result = validator.TestValidate(new RefreshTokenCommand(dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseQueryValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseQueryValidator();

        var result = validator.TestValidate(new GetCourseQuery(1, 10, null, null, null, null));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseProgressValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseProgressValidator();

        var result = validator.TestValidate(new GetCourseProgressQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseOverviewValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseOverviewValidator();

        var result = validator.TestValidate(new GetCourseOverviewQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseContentValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseContentValidator();

        var result = validator.TestValidate(new GetCourseContentQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseContentByIdValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseContentByIdValidator();

        var result = validator.TestValidate(new GetCourseContentByIdQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetCourseReviewsValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetCourseReviewsValidator();

        var result = validator.TestValidate(new GetCourseReviewsQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetUserCourseReviewValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetUserCourseReviewValidator();

        var result = validator.TestValidate(
            new GetUserCourseReviewQuery(Guid.NewGuid(), Guid.NewGuid())
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateReviewValidator_ShouldPass_WithValidInput()
    {
        var validator = new CreateReviewValidator();
        var dto = new CreateReviewRequestDto { Rating = 4, ReviewText = "Good course" };

        var result = validator.TestValidate(new CreateReviewCommand(Guid.NewGuid(), dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateReviewValidator_ShouldFail_WhenRatingOutOfRange()
    {
        var validator = new CreateReviewValidator();
        var dto = new CreateReviewRequestDto { Rating = 6, ReviewText = "Good" };

        var result = validator.TestValidate(new CreateReviewCommand(Guid.NewGuid(), dto));

        result.ShouldHaveValidationErrorFor(x => x.Dto.Rating);
    }

    [Fact]
    public void UpdateReviewValidator_ShouldPass_WithValidInput()
    {
        var validator = new UpdateReviewValidator();
        var dto = new UpdateReviewRequestDto { Rating = 3, ReviewText = "Updated review" };

        var result = validator.TestValidate(new UpdateReviewCommand(Guid.NewGuid(), dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeleteReviewValidator_ShouldPass_WithValidInput()
    {
        var validator = new DeleteReviewValidator();

        var result = validator.TestValidate(new DeleteReviewCommand(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateContentCompletionStatusValidator_ShouldPass_WithValidInput()
    {
        var validator = new UpdateContentCompletionStatusValidator();
        var request = new UpdateContentCompletionStatusRequest { IsCompleted = true };

        var result = validator.TestValidate(
            new UpdateContentCompletionStatusCommand(Guid.NewGuid(), request)
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetReferenceDataValidator_ShouldPass_WithValidInput()
    {
        var validator = new GetReferenceDataQueryValidator();

        var result = validator.TestValidate(new GetReferenceDataQuery("CourseCategory"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SubmitAssessmentValidator_ShouldPass_WithValidInput()
    {
        var validator = new SubmitAssessmentValidator();
        var dto = new AssessmentSubmitRequestDto
        {
            UserId = Guid.NewGuid(),
            StartedOn = DateTime.UtcNow.AddMinutes(-5),
            Answer = new List<Answer>
            {
                new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "A" },
            },
        };

        var result = validator.TestValidate(new SubmitAssessmentCommand(Guid.NewGuid(), dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SubmitAssessmentValidator_ShouldPass_WhenNoAnswers()
    {
        var validator = new SubmitAssessmentValidator();
        var dto = new AssessmentSubmitRequestDto
        {
            UserId = Guid.NewGuid(),
            StartedOn = DateTime.UtcNow.AddMinutes(-5),
            Answer = new List<Answer>(),
        };

        var result = validator.TestValidate(new SubmitAssessmentCommand(Guid.NewGuid(), dto));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SubmitAssessmentValidator_ShouldFail_WhenStartedOnInFuture()
    {
        var validator = new SubmitAssessmentValidator();
        var dto = new AssessmentSubmitRequestDto
        {
            UserId = Guid.NewGuid(),
            StartedOn = DateTime.UtcNow.AddMinutes(5),
            Answer = new List<Answer>
            {
                new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "A" },
            },
        };

        var result = validator.TestValidate(new SubmitAssessmentCommand(Guid.NewGuid(), dto));

        result.ShouldHaveValidationErrorFor(x => x.Dto.StartedOn);
    }
}
