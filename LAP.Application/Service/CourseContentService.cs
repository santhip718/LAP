using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="ICourseContentService"/> orchestrating course content operations.
/// </summary>
public class CourseContentService : ICourseContentService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<CourseContentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CourseContentService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper.</param>
    /// <param name="logger">The custom logger.</param>
    public CourseContentService(
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<CourseContentService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a course content item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course content.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The course content item if found; otherwise, null.</returns>
    public async Task<CourseContent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving course content {ContentId}.", id);
        return await _repositoryWrapper.CourseContent.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Adds a new course content item.
    /// </summary>
    /// <param name="courseContent">The course content entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The added course content item.</returns>
    public async Task<CourseContent> AddAsync(
        CourseContent courseContent,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding course content {ContentId}.", courseContent.Id);
        return await _repositoryWrapper.CourseContent.AddAsync(courseContent, cancellationToken);
    }

    /// <summary>
    /// Updates an existing course content item.
    /// </summary>
    /// <param name="courseContent">The course content entity with updated values.</param>
    public void Update(CourseContent courseContent)
    {
        _logger.LogDebug("Updating course content {ContentId}.", courseContent.Id);
        _repositoryWrapper.CourseContent.Update(courseContent);
    }

    /// <summary>
    /// Deletes a course content item.
    /// </summary>
    /// <param name="courseContent">The course content entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting course content {ContentId}.", id);
        return await _repositoryWrapper.CourseContent.SoftDeleteAsync(
            c => c.Id == id,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves a meta topic by the course identifier and meta topic name.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="name">The name of the meta topic.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching meta topic if found; otherwise, null.</returns>
    public async Task<CourseMetaTopic?> GetMetaTopicByCourseAndNameAsync(
        Guid courseId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Retrieving meta topic by course {CourseId} and name {Name}.",
            courseId,
            name
        );
        return await _repositoryWrapper
            .Repository<CourseMetaTopic>()
            .FindFirstByConditionAsync(
                mt => mt.IsActive && mt.CourseId == courseId && mt.Name == name,
                cancellationToken
            );
    }

    /// <summary>
    /// Retrieves a course content item with its meta topic and content type.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course content item if found; otherwise, null.</returns>
    public async Task<CourseContent?> GetContentWithMetaTopicAsync(
        Guid contentId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Fetching content {ContentId} with meta topic.", contentId);

        return await _repositoryWrapper
            .CourseContent.FindByCondition(cc => cc.IsActive && cc.Id == contentId)
            .Include(cc => cc.MetaTopic)
            .Include(cc => cc.ContentType)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the previous content item in the course sequence.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="metaSequenceOrder">The sequence order of the meta topic.</param>
    /// <param name="sequenceOrder">The sequence order of the content within the meta topic.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The previous course content item if found; otherwise, null.</returns>
    public async Task<CourseContent?> GetPreviousContentAsync(
        Guid courseId,
        int metaSequenceOrder,
        int sequenceOrder,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Fetching previous content for course {CourseId}.", courseId);

        return await _repositoryWrapper
            .CourseContent.FindByCondition(cc => cc.IsActive && cc.MetaTopic.CourseId == courseId)
            .Include(cc => cc.MetaTopic)
            .OrderByDescending(cc => cc.MetaTopic.SequenceOrder)
            .ThenByDescending(cc => cc.SequenceOrder)
            .FirstOrDefaultAsync(
                cc =>
                    cc.MetaTopic.SequenceOrder < metaSequenceOrder
                    || (
                        cc.MetaTopic.SequenceOrder == metaSequenceOrder
                        && cc.SequenceOrder < sequenceOrder
                    ),
                cancellationToken
            );
    }

    /// <summary>
    /// Adds a new meta topic.
    /// </summary>
    /// <param name="metaTopic">The meta topic entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddMetaTopicAsync(
        CourseMetaTopic metaTopic,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding meta topic {MetaTopicId}.", metaTopic.Id);
        await _repositoryWrapper
            .Repository<CourseMetaTopic>()
            .AddAsync(metaTopic, cancellationToken);
    }

    /// <summary>
    /// Retrieves the largest sequence order value among meta topics for a course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The maximum sequence order of meta topics for the course.</returns>
    public async Task<int> GetLargestMetaTopicSequenceOrderByCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Retrieving largest meta topic sequence order for course {CourseId}.",
            courseId
        );
        return await _repositoryWrapper
            .Repository<CourseMetaTopic>()
            .GetByConditionNoTracking(mt => mt.CourseId == courseId)
            .OrderByDescending(mt => mt.SequenceOrder)
            .Select(mt => mt.SequenceOrder)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the largest sequence order value among content items for a meta topic.
    /// </summary>
    /// <param name="metaTopicId">The unique identifier of the meta topic to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The maximum sequence order of content items within the meta topic.</returns>
    public async Task<int> GetLargestContentSequenceOrderByMetaTopicAsync(
        Guid metaTopicId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Retrieving largest content sequence order for meta topic {MetaTopicId}.",
            metaTopicId
        );
        return await _repositoryWrapper
            .CourseContent.GetByConditionNoTracking(cc => cc.MetaTopicId == metaTopicId)
            .OrderByDescending(cc => cc.SequenceOrder)
            .Select(cc => cc.SequenceOrder)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the next content item in the course sequence.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="metaSequenceOrder">The sequence order of the meta topic.</param>
    /// <param name="sequenceOrder">The sequence order of the content within the meta topic.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The next course content item if found; otherwise, null.</returns>
    public async Task<CourseContent?> GetNextContentAsync(
        Guid courseId,
        int metaSequenceOrder,
        int sequenceOrder,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Fetching next content for course {CourseId}.", courseId);

        return await _repositoryWrapper
            .CourseContent.FindByCondition(cc => cc.IsActive && cc.MetaTopic.CourseId == courseId)
            .Include(cc => cc.MetaTopic)
            .OrderBy(cc => cc.MetaTopic.SequenceOrder)
            .ThenBy(cc => cc.SequenceOrder)
            .FirstOrDefaultAsync(
                cc =>
                    cc.MetaTopic.SequenceOrder > metaSequenceOrder
                    || (
                        cc.MetaTopic.SequenceOrder == metaSequenceOrder
                        && cc.SequenceOrder > sequenceOrder
                    ),
                cancellationToken
            );
    }

    /// <summary>
    /// Retrieves a user's enrollment in a specific course.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The enrollment if found; otherwise, null.</returns>
    public async Task<Enrollment?> GetEnrollmentByUserAndCourseAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Fetching enrollment for user {UserId} and course {CourseId}.",
            userId,
            courseId
        );

        return await _repositoryWrapper.Enrollment.FindFirstByConditionAsync(
            e => e.IsActive && e.UserId == userId && e.CourseId == courseId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves the progress record for a specific enrollment and content item.
    /// </summary>
    /// <param name="enrollmentId">The enrollment identifier.</param>
    /// <param name="contentId">The content identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The user course progress record if found; otherwise, null.</returns>
    public async Task<UserCourseProgress?> GetProgressAsync(
        Guid enrollmentId,
        Guid contentId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Fetching progress for enrollment {EnrollmentId} and content {ContentId}.",
            enrollmentId,
            contentId
        );

        return await _repositoryWrapper.UserCourseProgress.FindFirstByConditionAsync(
            p => p.IsActive && p.EnrollmentId == enrollmentId && p.CourseContentId == contentId,
            cancellationToken
        );
    }

    /// <summary>
    /// Gets the total number of active content items in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The total number of active content items.</returns>
    public async Task<int> GetTotalContentCountAsync(
        Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Counting total contents for course {CourseId}.", courseId);

        return await _repositoryWrapper
            .CourseContent.FindByCondition(cc => cc.IsActive && cc.MetaTopic.CourseId == courseId)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the number of completed content items for a given enrollment.
    /// </summary>
    /// <param name="enrollmentId">The enrollment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of completed content items.</returns>
    public async Task<int> GetCompletedContentCountAsync(
        Guid enrollmentId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Counting completed contents for enrollment {EnrollmentId}.",
            enrollmentId
        );

        return await _repositoryWrapper
            .UserCourseProgress.FindByCondition(p =>
                p.IsActive && p.EnrollmentId == enrollmentId && p.IsCompleted
            )
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new user course progress record.
    /// </summary>
    /// <param name="progress">The user course progress entity.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddProgressAsync(
        UserCourseProgress progress,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Adding progress for enrollment {EnrollmentId} and content {ContentId}.",
            progress.EnrollmentId,
            progress.CourseContentId
        );

        await _repositoryWrapper.UserCourseProgress.CreateAsync(progress, cancellationToken);
    }

    /// <summary>
    /// Updates an existing user course progress record.
    /// </summary>
    /// <param name="progress">The user course progress entity.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateProgressAsync(
        UserCourseProgress progress,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Updating progress {ProgressId}.", progress.Id);

        _repositoryWrapper.UserCourseProgress.Update(progress);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the enrollment progress percentage.
    /// </summary>
    /// <param name="enrollmentId">The enrollment identifier.</param>
    /// <param name="percentage">The new progress percentage.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateEnrollmentProgressAsync(
        Guid enrollmentId,
        decimal percentage,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Updating enrollment {EnrollmentId} progress to {Percentage}%.",
            enrollmentId,
            percentage
        );

        Enrollment? enrollment = await _repositoryWrapper.Enrollment.FindFirstByConditionAsync(
            e => e.IsActive && e.Id == enrollmentId,
            cancellationToken
        );

        if (enrollment != null)
        {
            enrollment.ProgressPercentage = percentage;
            enrollment.CompletedOn = percentage >= 100 ? DateTime.UtcNow : null;
            _repositoryWrapper.Enrollment.Update(enrollment);
        }
    }

    /// <summary>
    /// Persists pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Saving changes.");
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }
}
