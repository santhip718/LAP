using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="ILeaderboardService"/> using <see cref="IRepositoryWrapper"/>.
/// </summary>
public class LeaderboardService : ILeaderboardService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<LeaderboardService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public LeaderboardService(
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<LeaderboardService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active users for the overall leaderboard.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of all active users ordered by leaderboard position.</returns>
    public async Task<List<User>> GetOverallLeaderboardAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving overall leaderboard");
        return await _repositoryWrapper
            .User.FindByCondition(u => u.IsActive)
            .Include(u => u.Person)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves completed assessment histories for a specific course leaderboard.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of completed assessment histories for the specified course.</returns>
    public async Task<List<AssessmentHistory>> GetLeaderboardByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving leaderboard for course {CourseId}", courseId);
        return await _repositoryWrapper
            .Leaderboard.FindByCondition(ah =>
                ah.IsActive && ah.Assessment.CourseId == courseId && ah.CompletedOn != null
            )
            .Include(ah => ah.User.Person)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks whether a course exists by its identifier.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if the course exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> CourseExistsAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking existence of course {CourseId}.", courseId);
        bool exist = await _repositoryWrapper
            .Repository<Course>()
            .AnyByConditionAsync(x => x.IsActive && x.Id == courseId, cancellationToken);
        _logger.LogDebug("Course {CourseId} existence: {Exists}", courseId, exist);
        return exist;
    }
}
