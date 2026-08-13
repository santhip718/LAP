using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for enrollment-related operations.
/// </summary>
public interface IEnrollmentService
{
    /// <summary>
    /// Retrieves all enrollments.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all enrollments.</returns>
    Task<List<Enrollment>> GetAllEnrollmentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user is already enrolled in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the user is already enrolled; otherwise, <c>false</c>.</returns>
    Task<bool> IsUserEnrolledAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a filtered list of enrollments by course name, category, and user.
    /// </summary>
    /// <param name="courseName">Optional course name filter.</param>
    /// <param name="categoryId">Optional category identifier filter.</param>
    /// <param name="userId">Optional user identifier filter.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of matching enrollments.</returns>
    Task<List<Enrollment>> GetEnrollmentAsync(
        string? courseName,
        Guid? categoryId,
        Guid? userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves an enrollment by its identifier with full details.
    /// </summary>
    /// <param name="id">The enrollment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The enrollment if found; otherwise, <c>null</c>.</returns>
    Task<Enrollment?> GetEnrollmentByIdWithDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new enrollment and returns the created entity.
    /// </summary>
    /// <param name="enrollment">The enrollment entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created enrollment.</returns>
    Task<Enrollment> AddEnrollmentAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing enrollment.
    /// </summary>
    /// <param name="enrollment">The enrollment entity to update.</param>
    void UpdateEnrollment(Enrollment enrollment);
}
