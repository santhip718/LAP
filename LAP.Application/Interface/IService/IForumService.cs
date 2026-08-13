using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for forum-related operations.
/// </summary>
public interface IForumService
{
    /// <summary>
    /// Checks whether a course exists.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the course exists; otherwise, <c>false</c>.</returns>
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all forum messages for a specific course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of forum messages for the course.</returns>
    Task<List<ForumMessage>> GetMessageByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new forum message and returns the created entity.
    /// </summary>
    /// <param name="message">The forum message entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created forum message.</returns>
    Task<ForumMessage> AddMessageAsync(ForumMessage message, CancellationToken cancellationToken = default);

}
