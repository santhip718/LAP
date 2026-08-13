using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for leaderboard-related operations.
/// </summary>
public interface ILeaderboardService
{
    /// <summary>Retrieves all active users for the overall leaderboard.</summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all active users.</returns>
    Task<List<User>> GetOverallLeaderboardAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves completed assessment histories for a specific course leaderboard.</summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of completed assessment history records.</returns>
    Task<List<AssessmentHistory>> GetLeaderboardByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Checks whether a course exists by its identifier.</summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the course exists; otherwise, <c>false</c>.</returns>
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
}
