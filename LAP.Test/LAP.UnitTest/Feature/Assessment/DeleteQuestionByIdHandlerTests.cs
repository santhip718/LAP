using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class DeleteQuestionByIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<DeleteQuestionByIdHandler>> _loggerMock;
    private readonly DeleteQuestionByIdHandler _handler;

    public DeleteQuestionByIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<DeleteQuestionByIdHandler>>();
        _handler = new DeleteQuestionByIdHandler(_assessmentServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteQuestion_WhenQuestionExists()
    {
        var questionId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = questionId,
            AssessmentId = assessmentId,
            QuestionText = "Test Question",
        };

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);
        _assessmentServiceMock
            .Setup(x => x.DeleteQuestionAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LAP.Domain.Entity.Assessment { Id = assessmentId, TotalMark = 10 });
        _assessmentServiceMock
            .Setup(x =>
                x.CountActiveQuestionByAssessmentIdAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(9);
        _assessmentServiceMock
            .Setup(x =>
                x.UpdateAssessmentAsync(
                    It.IsAny<LAP.Domain.Entity.Assessment>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new DeleteQuestionByIdCommand(questionId),
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task Handle_ShouldRecalculateTotalMarks_AfterDeletion()
    {
        var questionId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = questionId,
            AssessmentId = assessmentId,
            QuestionText = "Test Question",
        };
        var assessment = new LAP.Domain.Entity.Assessment { Id = assessmentId, TotalMark = 10 };

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);
        _assessmentServiceMock
            .Setup(x => x.DeleteQuestionAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.CountActiveQuestionByAssessmentIdAsync(
                    assessmentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(9);
        _assessmentServiceMock
            .Setup(x =>
                x.UpdateAssessmentAsync(
                    It.IsAny<LAP.Domain.Entity.Assessment>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteQuestionByIdCommand(questionId), CancellationToken.None);

        _assessmentServiceMock.Verify(
            x =>
                x.UpdateAssessmentAsync(
                    It.Is<LAP.Domain.Entity.Assessment>(a =>
                        a.Id == assessmentId && a.TotalMark == 9
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenQuestionNotFound()
    {
        var questionId = Guid.NewGuid();

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Question?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteQuestionByIdCommand(questionId), CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenNoRowsAffected()
    {
        var questionId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = questionId,
            AssessmentId = assessmentId,
            QuestionText = "Test Question",
        };

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);
        _assessmentServiceMock
            .Setup(x => x.DeleteQuestionAsync(questionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteQuestionByIdCommand(questionId), CancellationToken.None)
        );
    }
}
