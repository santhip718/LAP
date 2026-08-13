using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.AssessmentHandlers;

public class GetAssessmentResultHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<GetAssessmentResultHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAssessmentResultHandler _handler;

    public GetAssessmentResultHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<GetAssessmentResultHandler>>();
        _requestContextMock = new Mock<IRequestContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAssessmentResultHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnResult()
    {
        var assessmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetAssessmentResultQuery(assessmentId);
        var assessment = new Assessment
        {
            Id = assessmentId,
            Title = "Test Assessment",
            PassingMark = 50,
        };
        var history = new AssessmentHistory
        {
            Score = 80,
            WeightedScore = 75,
            StartedOn = DateTime.UtcNow.AddHours(-1),
            CompletedOn = DateTime.UtcNow,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.GetAllAssessmentHistoriesAsync(
                    assessmentId,
                    userId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<AssessmentHistory> { history });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(assessmentId, result.AssessmentId);
        Assert.Equal("Test Assessment", result.AssessmentTitle);
        Assert.Single(result.Attempts);
        Assert.Equal(80, result.Attempts[0].Score);
        Assert.True(result.Attempts[0].Passed);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAssessmentNotFound()
    {
        var query = new GetAssessmentResultQuery(Guid.NewGuid());

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Assessment?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Assessment not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoHistoryFound()
    {
        var assessmentId = Guid.NewGuid();
        var query = new GetAssessmentResultQuery(assessmentId);

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Assessment { Id = assessmentId });
        _assessmentServiceMock
            .Setup(x =>
                x.GetAllAssessmentHistoriesAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Enumerable.Empty<AssessmentHistory>());

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Result not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotPassed_WhenScoreBelowPassingMark()
    {
        var assessmentId = Guid.NewGuid();
        var query = new GetAssessmentResultQuery(assessmentId);

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _assessmentServiceMock
            .Setup(x =>
                x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Assessment { Id = assessmentId, PassingMark = 70 });
        _assessmentServiceMock
            .Setup(x =>
                x.GetAllAssessmentHistoriesAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new List<AssessmentHistory>
                {
                    new AssessmentHistory
                    {
                        Score = 60,
                        StartedOn = DateTime.UtcNow.AddHours(-1),
                        CompletedOn = DateTime.UtcNow,
                    },
                }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Attempts[0].Passed);
    }
}
