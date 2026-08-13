using System.Linq.Expressions;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.AssessmentHandlers;

public class SubmitAssessmentHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<SubmitAssessmentHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly SubmitAssessmentHandler _handler;

    public SubmitAssessmentHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<SubmitAssessmentHandler>>();
        _requestContextMock = new Mock<IRequestContext>();

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<SubmitAssessmentResponseDto>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((Func<Task<SubmitAssessmentResponseDto>> op, CancellationToken _) => op());

        _handler = new SubmitAssessmentHandler(
            _assessmentServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldSubmitAssessment()
    {
        var assessmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var answer = new List<Answer>
        {
            new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "A" },
            new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "B" },
        };
        var dto = new AssessmentSubmitRequestDto
        {
            UserId = userId,
            StartedOn = DateTime.UtcNow.AddMinutes(-30),
            Answer = answer,
        };
        var command = new SubmitAssessmentCommand(assessmentId, dto);

        var assessment = new Assessment
        {
            Id = assessmentId,
            CourseId = courseId,
            PassingMark = 1,
            Questions = new List<Question>
            {
                new()
                {
                    Id = answer[0].QuestionId,
                    Answer = "A",
                    Weight = 1,
                },
                new()
                {
                    Id = answer[1].QuestionId,
                    Answer = "B",
                    Weight = 1,
                },
            },
        };

        var history = new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            WeightedScore = 100,
            CompletedOn = DateTime.UtcNow
        };
        var tiers = new List<RefTerm>
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Logic Architect" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Runtime Titan" },
            new RefTerm { Id = Guid.NewGuid(), Name = "System Sovereign" },
        };
        var tier = tiers[0];
        var user = new User { Id = userId };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentHistoryAsync(It.IsAny<AssessmentHistory>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(history);
        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentAnswerRangeAsync(
                    It.IsAny<IEnumerable<AssessmentAnswer>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        _assessmentServiceMock
            .Setup(x =>
                x.GetUserCourseAssessmentHistoriesAsync(
                    userId,
                    courseId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<AssessmentHistory>());
        _assessmentServiceMock
            .Setup(x =>
                x.GetUserAllCompletedAssessmentHistoryAsync(
                    userId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<AssessmentHistory> { history });
        _assessmentServiceMock
            .Setup(x =>
                x.GetTierAsync(
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(tiers);
        _assessmentServiceMock
            .Setup(x =>
                x.GetTierByScoreAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(tier);
        _assessmentServiceMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _assessmentServiceMock
            .Setup(x => x.UpdateUser(It.IsAny<User>()));
        _assessmentServiceMock
            .Setup(x => x.GetMetaTopicByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CourseMetaTopic>());

        _transactionServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(assessmentId, result.AssessmentId);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal(2, result.TotalQuestion);
        Assert.Equal(2, result.CorrectAnswer);
        Assert.Equal(2, result.Score);
        Assert.Equal("Code Cadet", result.TierAwarded);
        Assert.Equal("Completed", result.Status);
        Assert.NotNull(result.WeakTopic);
        Assert.Empty(result.WeakTopic);
        Assert.NotNull(result.Answers);
        Assert.Equal(2, result.Answers.Count);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotAuthenticated()
    {
        var command = new SubmitAssessmentCommand(Guid.NewGuid(), new AssessmentSubmitRequestDto());

        _requestContextMock.Setup(x => x.UserId).Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAssessmentNotFound()
    {
        var command = new SubmitAssessmentCommand(Guid.NewGuid(), new AssessmentSubmitRequestDto());

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Assessment?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Assessment not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotEnrolled()
    {
        var assessmentId = Guid.NewGuid();
        var dto = new AssessmentSubmitRequestDto { Answer = new List<Answer>() };
        var command = new SubmitAssessmentCommand(assessmentId, dto);
        var assessment = new Assessment { Id = assessmentId, CourseId = Guid.NewGuid() };

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Not enrolled", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoQuestions()
    {
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new AssessmentSubmitRequestDto { Answer = new List<Answer>() };
        var command = new SubmitAssessmentCommand(assessmentId, dto);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Assessment
                {
                    Id = assessmentId,
                    CourseId = Guid.NewGuid(),
                    Questions = new List<Question>(),
                }
            );
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("No questions", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenInvalidQuestionIds()
    {
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var answer = new List<Answer>
        {
            new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "A" },
        };
        var dto = new AssessmentSubmitRequestDto
        {
            StartedOn = DateTime.UtcNow.AddMinutes(-10),
            Answer = answer,
        };
        var command = new SubmitAssessmentCommand(assessmentId, dto);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Assessment
                {
                    Id = assessmentId,
                    CourseId = Guid.NewGuid(),
                    Questions = new List<Question>
                    {
                        new() { Id = Guid.NewGuid(), Answer = "B" },
                    },
                }
            );
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Invalid questions", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenStartedOnInFuture()
    {
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var answer = new List<Answer>
        {
            new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "A" },
        };
        var dto = new AssessmentSubmitRequestDto
        {
            StartedOn = DateTime.UtcNow.AddMinutes(10),
            Answer = answer,
        };
        var command = new SubmitAssessmentCommand(assessmentId, dto);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Assessment
                {
                    Id = assessmentId,
                    CourseId = Guid.NewGuid(),
                    Questions = new List<Question>
                    {
                        new()
                        {
                            Id = answer[0].QuestionId,
                            Answer = "A",
                            Weight = 1,
                        },
                    },
                }
            );
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Invalid started time", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCalculatePartialScore()
    {
        var assessmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var q1Id = Guid.NewGuid();
        var q2Id = Guid.NewGuid();
        var answer = new List<Answer>
        {
            new() { QuestionId = q1Id, SelectedAnswer = "A" },
            new() { QuestionId = q2Id, SelectedAnswer = "Wrong" },
        };
        var dto = new AssessmentSubmitRequestDto
        {
            UserId = userId,
            StartedOn = DateTime.UtcNow.AddMinutes(-30),
            Answer = answer,
        };
        var command = new SubmitAssessmentCommand(assessmentId, dto);

        var assessment = new Assessment
        {
            Id = assessmentId,
            CourseId = courseId,
            PassingMark = 2,
            Questions = new List<Question>
            {
                new()
                {
                    Id = q1Id,
                    Answer = "A",
                    Weight = 2,
                },
                new()
                {
                    Id = q2Id,
                    Answer = "B",
                    Weight = 3,
                },
            },
        };

        var history = new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            WeightedScore = 100,
            CompletedOn = DateTime.UtcNow
        };
        var tiers = new List<RefTerm>
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Logic Architect" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Runtime Titan" },
            new RefTerm { Id = Guid.NewGuid(), Name = "System Sovereign" },
        };
        var tier = tiers[0];
        var user = new User { Id = userId };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentWithQuestionsAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.IsUserEnrolledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentHistoryAsync(It.IsAny<AssessmentHistory>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(history);
        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentAnswerRangeAsync(
                    It.IsAny<IEnumerable<AssessmentAnswer>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        _assessmentServiceMock
            .Setup(x =>
                x.GetUserCourseAssessmentHistoriesAsync(
                    userId,
                    courseId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<AssessmentHistory>());
        _assessmentServiceMock
            .Setup(x =>
                x.GetUserAllCompletedAssessmentHistoryAsync(
                    userId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<AssessmentHistory> { history });
        _assessmentServiceMock
            .Setup(x =>
                x.GetTierAsync(
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(tiers);
        _assessmentServiceMock
            .Setup(x =>
                x.GetTierByScoreAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(tier);
        _assessmentServiceMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _assessmentServiceMock
            .Setup(x => x.UpdateUser(It.IsAny<User>()));
        _assessmentServiceMock
            .Setup(x => x.GetMetaTopicByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CourseMetaTopic>());

        _transactionServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.CorrectAnswer);
        Assert.Equal(2, result.Score);
        Assert.Equal(40, result.WeightedScore);
        Assert.Equal("Completed", result.Status);
        Assert.NotNull(result.WeakTopic);
        Assert.Empty(result.WeakTopic);
        Assert.NotNull(result.Answers);
        Assert.Equal(2, result.Answers.Count);
        Assert.Equal(q1Id, result.Answers.ElementAt(0).QuestionId);
        Assert.True(result.Answers.ElementAt(0).IsCorrect);
        Assert.Equal(2, result.Answers.ElementAt(0).ObtainedScore);
        Assert.Equal(q2Id, result.Answers.ElementAt(1).QuestionId);
        Assert.False(result.Answers.ElementAt(1).IsCorrect);
        Assert.Equal(0, result.Answers.ElementAt(1).ObtainedScore);
    }
}
