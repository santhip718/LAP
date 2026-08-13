using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class DeleteAssessmentByIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<DeleteAssessmentByIdHandler>> _loggerMock;
    private readonly DeleteAssessmentByIdHandler _handler;

    public DeleteAssessmentByIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<DeleteAssessmentByIdHandler>>();
        _handler = new DeleteAssessmentByIdHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldDeleteAssessment_WhenAssessmentExists()
    {
        var id = Guid.NewGuid();

        _assessmentServiceMock
            .Setup(x => x.DeleteAssessmentAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new DeleteAssessmentByIdCommand(id),
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenNoRowsAffected()
    {
        var id = Guid.NewGuid();

        _assessmentServiceMock
            .Setup(x => x.DeleteAssessmentAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteAssessmentByIdCommand(id), CancellationToken.None)
        );
    }
}
