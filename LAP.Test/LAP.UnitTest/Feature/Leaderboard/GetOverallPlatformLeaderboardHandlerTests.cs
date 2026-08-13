using AutoMapper;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Leaderboard.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Handler.Leaderboard;

public class GetOverallPlatformLeaderboardHandlerTest
{
    private readonly Mock<ILeaderboardService> _leaderboardServiceMock;
    private readonly Mock<ICustomLogger<GetOverallPlatformLeaderboardHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetOverallPlatformLeaderboardHandler _handler;

    public GetOverallPlatformLeaderboardHandlerTest()
    {
        _leaderboardServiceMock = new Mock<ILeaderboardService>();
        _loggerMock = new Mock<ICustomLogger<GetOverallPlatformLeaderboardHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetOverallPlatformLeaderboardHandler(
            _leaderboardServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnTop25Users_WhenUsersExist()
    {
        var users = new List<User>();
        for (int i = 0; i < 30; i++)
        {
            users.Add(new User { Id = Guid.NewGuid(), OverallWeightedScore = 100 - i });
        }

        _leaderboardServiceMock
            .Setup(x => x.GetOverallLeaderboardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var mappedDtos = users
            .Take(CommonConstants.DEFAULT_PAGE_SIZE)
            .Select(
                (u, i) =>
                    new LeaderboardDto
                    {
                        UserId = u.Id,
                        OverallWeightedScore = u.OverallWeightedScore,
                        Rank = i + 1,
                    }
            )
            .ToList();

        _mapperMock
            .Setup(x => x.Map<List<LeaderboardDto>>(It.IsAny<List<User>>()))
            .Returns(mappedDtos);

        var result = await _handler.Handle(
            new GetOverallPlatformLeaderboardQuery(CommonConstants.DEFAULT_PAGE_SIZE),
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.Equal(CommonConstants.DEFAULT_PAGE_SIZE, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(100, result[0].OverallWeightedScore);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoUsers()
    {
        _leaderboardServiceMock
            .Setup(x => x.GetOverallLeaderboardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());
        _mapperMock
            .Setup(x => x.Map<List<LeaderboardDto>>(It.IsAny<List<User>>()))
            .Returns(new List<LeaderboardDto>());

        var result = await _handler.Handle(
            new GetOverallPlatformLeaderboardQuery(CommonConstants.DEFAULT_PAGE_SIZE),
            CancellationToken.None
        );

        Assert.Empty(result);
    }
}
