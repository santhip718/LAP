using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for course-content and meta-topic operations.
/// </summary>
public interface ICourseContentService
{
    /// <summary>
    /// Retrieves a course content by its identifier.
    /// </summary>
    /// <param name="id">The course content identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course content if found; otherwise, <c>null</c>.</returns>
    Task<CourseContent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new course content and returns the created entity.
    /// </summary>
    /// <param name="courseContent">The course content entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created course content.</returns>
    Task<CourseContent> AddAsync(
        CourseContent courseContent,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing course content.
    /// </summary>
    /// <param name="courseContent">The course content entity to update.</param>
    void Update(CourseContent courseContent);

    /// <summary>
    /// Deletes a course content by its identifier.
    /// </summary>
    /// <param name="id">The course content identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a meta topic by course and name.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="name">The meta topic name.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The meta topic if found; otherwise, <c>null</c>.</returns>
    Task<CourseMetaTopic?> GetMetaTopicByCourseAndNameAsync(
        Guid courseId,
        string name,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new meta topic.
    /// </summary>
    /// <param name="metaTopic">The meta topic entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task AddMetaTopicAsync(
        CourseMetaTopic metaTopic,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the largest sequence order among meta topics for a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The largest sequence order, or 0 if none exist.</returns>
    Task<int> GetLargestMetaTopicSequenceOrderByCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the largest sequence order among content items for a meta topic.
    /// </summary>
    /// <param name="metaTopicId">The meta topic identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The largest sequence order, or 0 if none exist.</returns>
    Task<int> GetLargestContentSequenceOrderByMetaTopicAsync(
        Guid metaTopicId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a course content item with its meta topic and content type.
    /// </summary>
    Task<CourseContent?> GetContentWithMetaTopicAsync(
        Guid contentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the previous content item in the course sequence.
    /// </summary>
    Task<CourseContent?> GetPreviousContentAsync(
        Guid courseId,
        int metaSequenceOrder,
        int sequenceOrder,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the next content item in the course sequence.
    /// </summary>
    Task<CourseContent?> GetNextContentAsync(
        Guid courseId,
        int metaSequenceOrder,
        int sequenceOrder,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a user's enrollment in a specific course.
    /// </summary>
    Task<Enrollment?> GetEnrollmentByUserAndCourseAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the progress record for a specific enrollment and content item.
    /// </summary>
    Task<UserCourseProgress?> GetProgressAsync(
        Guid enrollmentId,
        Guid contentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the total number of active content items in a course.
    /// </summary>
    Task<int> GetTotalContentCountAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the number of completed content items for a given enrollment.
    /// </summary>
    Task<int> GetCompletedContentCountAsync(
        Guid enrollmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new user course progress record.
    /// </summary>
    Task AddProgressAsync(
        UserCourseProgress progress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing user course progress record.
    /// </summary>
    Task UpdateProgressAsync(
        UserCourseProgress progress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates the enrollment progress percentage.
    /// </summary>
    Task UpdateEnrollmentProgressAsync(
        Guid enrollmentId,
        decimal percentage,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Persists pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
