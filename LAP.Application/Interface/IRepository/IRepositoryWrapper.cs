using LAP.Domain.Entity;

namespace LAP.Application.Interface.IRepository;

/// <summary>
/// Provides a wrapper for all repositories in the system, following the Unit of Work pattern.
/// </summary>
public interface IRepositoryWrapper
{
    /// <summary>
    /// Gets the repository for managing users.
    /// </summary>
    IUserRepository User { get; }

    /// <summary>
    /// Gets the repository for managing refresh tokens.
    /// </summary>
    IRefreshTokenRepository RefreshToken { get; }

    /// <summary>
    /// Gets the repository for managing course reviews.
    /// </summary>
    IReviewRepository Review { get; }

    /// <summary>
    /// Gets the repository for managing courses.
    /// </summary>
    ICourseRepository Course { get; }

    /// <summary>
    /// Gets the repository for managing course contents.
    /// </summary>
    ICourseContentRepository CourseContent { get; }

    /// <summary>
    /// Gets the repository for forum message operations.
    /// </summary>
    IForumRepository Forum { get; }

    /// <summary>
    /// Gets the repository for managing user course progress.
    /// </summary>
    IUserCourseProgressRepository UserCourseProgress { get; }

    /// <summary>
    /// Gets the repository for managing enrollments.
    /// </summary>
    IEnrollmentRepository Enrollment { get; }

    /// <summary>
    /// Gets the repository for managing assessments.
    /// </summary>
    IAssessmentRepository Assessment { get; }

    /// <summary>
    /// Gets the repository for leaderboard queries.
    /// </summary>
    ILeaderboardRepository Leaderboard { get; }
    /// Gets the repository for managing role-feature mappings.
    /// </summary>
    IRoleFeatureMappingRepository RoleFeatureMapping { get; }

    /// <summary>
    /// Gets the repository for managing assessment history records.
    /// </summary>
    IAssessmentHistoryRepository AssessmentHistory { get; }

    /// <summary>
    /// Gets the repository for managing assessment answers.
    /// </summary>
    IAssessmentAnswerRepository AssessmentAnswer { get; }

    /// <summary>
    /// Gets the repository for managing tier reference data.
    /// </summary>
    ITierRepository Tier { get; }

    /// <summary>
    /// Gets a generic repository for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The type of the domain entity.</typeparam>
    /// <returns>A generic repository for the specified entity type.</returns>
    IBaseRepository<T> Repository<T>()
        where T : BaseEntity;

    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation within a database transaction.
    /// </summary>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes an asynchronous operation that returns a result within a database transaction.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default
    );
}
