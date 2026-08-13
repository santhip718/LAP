using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.AssessmentHandlers;

public class GetUserAssessmentHistoryHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<GetUserAssessmentHistoryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserAssessmentHistoryHandler _handler;

    public GetUserAssessmentHistoryHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<GetUserAssessmentHistoryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserAssessmentHistoryHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedHistory()
    {
        var userId = Guid.NewGuid();
        var query = new GetUserAssessmentHistoryQuery(userId, 1, 10);
        var user = new User { Id = userId };
        var histories = new List<AssessmentHistory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AssessmentId = Guid.NewGuid(),
                Score = 80,
                WeightedScore = 75,
                StartedOn = DateTime.UtcNow.AddHours(-1),
                CompletedOn = DateTime.UtcNow,
                Assessment = new Assessment { Title = "Test", PassingMark = 50 },
            },
        };

        _assessmentServiceMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _assessmentServiceMock
            .Setup(x =>
                x.GetPagedAssessmentHistoryAsync(
                    userId,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((histories, 1));

        _mapperMock
            .Setup(m => m.Map<List<AssessmentHistoryItemDto>>(It.IsAny<object>()))
            .Returns(new List<AssessmentHistoryItemDto> { new() { AssessmentTitle = "Test" } });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result.Item);
        Assert.Equal(1, result.TotalRecords);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var query = new GetUserAssessmentHistoryQuery(Guid.NewGuid(), 1, 10);

        _assessmentServiceMock
            .Setup(x =>
                x.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoHistory()
    {
        var userId = Guid.NewGuid();
        var query = new GetUserAssessmentHistoryQuery(userId, 1, 10);

        _assessmentServiceMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId });
        _assessmentServiceMock
            .Setup(x =>
                x.GetPagedAssessmentHistoryAsync(
                    userId,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<AssessmentHistory>(), 0));

        _mapperMock
            .Setup(m => m.Map<List<AssessmentHistoryItemDto>>(It.IsAny<object>()))
            .Returns(new List<AssessmentHistoryItemDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result.Item);
        Assert.Equal(0, result.TotalRecords);
    }

    [Fact]
    public async Task Handle_ShouldHandleNullAssessment()
    {
        var userId = Guid.NewGuid();
        var query = new GetUserAssessmentHistoryQuery(userId, 1, 10);

        _assessmentServiceMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId });
        _assessmentServiceMock
            .Setup(x =>
                x.GetPagedAssessmentHistoryAsync(
                    userId,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new List<AssessmentHistory>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            AssessmentId = Guid.NewGuid(),
                            Score = 80,
                            StartedOn = DateTime.UtcNow.AddHours(-1),
                            CompletedOn = DateTime.UtcNow,
                            Assessment = null,
                        },
                    },
                    1
                )
            );

        _mapperMock
            .Setup(m => m.Map<List<AssessmentHistoryItemDto>>(It.IsAny<object>()))
            .Returns(new List<AssessmentHistoryItemDto> { new() { AssessmentTitle = null } });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result.Item);
    }
}
