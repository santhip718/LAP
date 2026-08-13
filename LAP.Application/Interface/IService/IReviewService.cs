using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access orchestration for course reviews.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves all active reviews for a specific course, including user details.
    /// </summary>
    Task<List<Review>> GetReviewByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a paginated list of active reviews for a specific course.
    /// </summary>
    Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedReviewsByCourseIdAsync(
        Guid courseId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a review by a specific user for a specific course.
    /// </summary>
    Task<Review?> GetUserReviewForCourseAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a user is enrolled in a specific course.
    /// </summary>
    Task<bool> IsUserEnrolledAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a user's enrollment for a specific course, including approval status.
    /// </summary>
    Task<Enrollment?> GetUserEnrollmentAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a user has already reviewed a specific course.
    /// </summary>
    Task<bool> HasUserReviewedAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a course by its unique identifier.
    /// </summary>
    Task<Course?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a review by its unique identifier.
    /// </summary>
    Task<Review?> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new review to the database.
    /// </summary>
    Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing review in the database.
    /// </summary>
    Task UpdateReviewAsync(Review review, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a review in the database.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected by the deletion.</returns>
    Task<int> DeleteReviewAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the course entity in the database.
    /// </summary>
    Task UpdateCourseAsync(Course course, CancellationToken cancellationToken = default);
}
