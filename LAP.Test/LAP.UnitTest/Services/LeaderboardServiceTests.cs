using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Application.Service;
using LAP.Domain.Entity;
using Moq;
using System.Linq.Expressions;
using MockQueryable.Moq;
using MockQueryable;
using Microsoft.EntityFrameworkCore;

namespace LAP.UnitTest.Services;

public class LeaderboardServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<ILeaderboardRepository> _leaderboardRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IBaseRepository<Course>> _courseRepoMock;
    private readonly Mock<ICustomLogger<LeaderboardService>> _loggerMock;
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardServiceTest()
    {
        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _leaderboardRepoMock = new Mock<ILeaderboardRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _courseRepoMock = new Mock<IBaseRepository<Course>>();
        _loggerMock = new Mock<ICustomLogger<LeaderboardService>>();

        _repoWrapperMock.Setup(x => x.Leaderboard).Returns(_leaderboardRepoMock.Object);
        _repoWrapperMock.Setup(x => x.User).Returns(_userRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<Course>()).Returns(_courseRepoMock.Object);

        _leaderboardService = new LeaderboardService(_repoWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetOverallLeaderboardAsync_ShouldReturnActiveUsers()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), IsActive = true, Person = new Person { FullName = "User 1" } },
            new() { Id = Guid.NewGuid(), IsActive = true, Person = new Person { FullName = "User 2" } },
        };
        
        var mock = users.BuildMock();
        _userRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(mock);

        var result = await _leaderboardService.GetOverallLeaderboardAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetOverallLeaderboardAsync_ShouldReturnEmpty_WhenNoActiveUsers()
    {
        var mock = new List<User>().BuildMock();
        _userRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(mock);

        var result = await _leaderboardService.GetOverallLeaderboardAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeaderboardByCourseIdAsync_ShouldReturnHistories()
    {
        var courseId = Guid.NewGuid();
        var histories = new List<AssessmentHistory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                WeightedScore = 90,
                IsActive = true,
                CompletedOn = DateTime.UtcNow,
                User = new User { Person = new Person { FullName = "User 1" } }
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                WeightedScore = 85,
                IsActive = true,
                CompletedOn = DateTime.UtcNow,
                User = new User { Person = new Person { FullName = "User 2" } }
            },
        };
        
        var mock = histories.BuildMock();
        _leaderboardRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<AssessmentHistory, bool>>>()))
            .Returns(mock);

        var result = await _leaderboardService.GetLeaderboardByCourseIdAsync(courseId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetLeaderboardByCourseIdAsync_ShouldReturnEmpty_WhenNoHistories()
    {
        var mock = new List<AssessmentHistory>().BuildMock();
        _leaderboardRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<AssessmentHistory, bool>>>()))
            .Returns(mock);

        var result = await _leaderboardService.GetLeaderboardByCourseIdAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task CourseExistsAsync_ShouldReturnTrue_WhenCourseExists()
    {
        var courseId = Guid.NewGuid();
        _courseRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _leaderboardService.CourseExistsAsync(courseId);

        Assert.True(result);
    }

    [Fact]
    public async Task CourseExistsAsync_ShouldReturnFalse_WhenCourseDoesNotExist()
    {
        _courseRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _leaderboardService.CourseExistsAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
