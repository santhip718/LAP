using AutoMapper;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Leaderboard.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Handler.Leaderboard;

public class GetLeaderboardByCourseIdHandlerTest
{
    private readonly Mock<ILeaderboardService> _leaderboardServiceMock;
    private readonly Mock<ICustomLogger<GetLeaderboardByCourseIdHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetLeaderboardByCourseIdHandler _handler;

    public GetLeaderboardByCourseIdHandlerTest()
    {
        _leaderboardServiceMock = new Mock<ILeaderboardService>();
        _loggerMock = new Mock<ICustomLogger<GetLeaderboardByCourseIdHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetLeaderboardByCourseIdHandler(
            _leaderboardServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnTop25Entries_WhenHistoriesExist()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var histories = new List<AssessmentHistory>();
        for (int i = 0; i < 30; i++)
        {
            histories.Add(
                new AssessmentHistory
                {
                    UserId = Guid.NewGuid(),
                    WeightedScore = 100 - i,
                    StartedOn = DateTime.UtcNow.AddHours(-1),
                    CompletedOn = DateTime.UtcNow.AddMinutes(-(i * 2)),
                }
            );
        }

        _leaderboardServiceMock
            .Setup(x => x.GetLeaderboardByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(histories);

        var mappedDtos = histories
            .Take(CommonConstants.DEFAULT_PAGE_SIZE)
            .Select(
                (h, i) =>
                    new LeaderboardDto
                    {
                        UserId = h.UserId,
                        OverallWeightedScore = h.WeightedScore,
                        Rank = i + 1,
                    }
            )
            .ToList();

        _mapperMock
            .Setup(x => x.Map<List<LeaderboardDto>>(It.IsAny<List<AssessmentHistory>>()))
            .Returns(mappedDtos);

        var result = await _handler.Handle(
            new GetLeaderboardByCourseIdQuery(courseId, CommonConstants.DEFAULT_PAGE_SIZE),
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.Equal(CommonConstants.DEFAULT_PAGE_SIZE, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(CommonConstants.DEFAULT_PAGE_SIZE, result[^1].Rank);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoHistories()
    {
        _leaderboardServiceMock
            .Setup(x =>
                x.GetLeaderboardByCourseIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<AssessmentHistory>());
        _mapperMock
            .Setup(x => x.Map<List<LeaderboardDto>>(It.IsAny<List<AssessmentHistory>>()))
            .Returns(new List<LeaderboardDto>());

        var result = await _handler.Handle(
            new GetLeaderboardByCourseIdQuery(Guid.NewGuid()),
            CancellationToken.None
        );

        Assert.Empty(result);
    }
}
