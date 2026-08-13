using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="IForumService"/> using <see cref="IRepositoryWrapper"/>
/// </summary>
public class ForumService : IForumService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<ForumService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public ForumService(IRepositoryWrapper repositoryWrapper, ICustomLogger<ForumService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Checks whether a course with the given identifier exists.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if the course exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> CourseExistsAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking if course {CourseId} exists.", courseId);

        return await _repositoryWrapper.Course.AnyByConditionNoTrackingAsync(
            c => c.Id == courseId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves all forum messages for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course whose forum messages to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of forum messages belonging to the specified course.</returns>
    public async Task<List<ForumMessage>> GetMessageByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving forum messages for course {CourseId}.", courseId);

        return await _repositoryWrapper
            .Forum.GetByConditionNoTracking(fm => fm.CourseId == courseId)
            .Include(fm => fm.User)
                .ThenInclude(u => u.Person)
            .OrderBy(fm => fm.DateCreated)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new forum message and returns the created entity.
    /// </summary>
    /// <param name="message">The forum message entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created forum message entity.</returns>
    public async Task<ForumMessage> AddMessageAsync(
        ForumMessage message,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Adding forum message {MessageId} for course {CourseId}.",
            message.Id,
            message.CourseId
        );

        return await _repositoryWrapper.Forum.AddAsync(message, cancellationToken);
    }
}
