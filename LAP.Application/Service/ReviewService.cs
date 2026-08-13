using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="IReviewService"/> providing direct data-access calls for reviews.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<ReviewService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper.</param>
    /// <param name="logger">The custom logger.</param>
    public ReviewService(IRepositoryWrapper repositoryWrapper, ICustomLogger<ReviewService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active reviews for a specific course, including user details.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of reviews for the course.</returns>
    public async Task<List<Review>> GetReviewByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching reviews for course {CourseId}.", courseId);
        return await _repositoryWrapper
            .Review.FindByCondition(r => r.IsActive && r.CourseId == courseId)
            .Include(r => r.User)
                .ThenInclude(u => u.Person)
            .OrderByDescending(r => r.DateCreated)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of active reviews for a specific course, including user details.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A tuple containing the review collection and total count.</returns>
    public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedReviewsByCourseIdAsync(
        Guid courseId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching paged reviews for course {CourseId} - page {Page}, size {Size}.",
            courseId,
            pageNumber,
            pageSize
        );

        IQueryable<Review> query = _repositoryWrapper
            .Review.FindByCondition(r => r.IsActive && r.CourseId == courseId)
            .Include(r => r.User)
                .ThenInclude(u => u.Person);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Review> items = await query
            .OrderByDescending(r => r.DateCreated)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved {Count} reviews for course {CourseId}.", items.Count, courseId);

        return (items, totalCount);
    }

    /// <summary>
    /// Retrieves a review by a specific user for a specific course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The review if found; otherwise, null.</returns>
    public async Task<Review?> GetUserReviewForCourseAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching review for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );
        return await _repositoryWrapper
            .Review.FindByCondition(r => r.IsActive && r.CourseId == courseId && r.UserId == userId)
            .Include(r => r.User)
                .ThenInclude(u => u.Person)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a user is enrolled in a specific course.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the user is enrolled; otherwise, false.</returns>
    public async Task<bool> IsUserEnrolledAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Checking enrollment for user {UserId} in course {CourseId}.",
            userId,
            courseId
        );
        return await _repositoryWrapper
            .Repository<Enrollment>()
            .AnyByConditionAsync(
                e => e.IsActive && e.UserId == userId && e.CourseId == courseId,
                cancellationToken
            );
    }

    /// <summary>
    /// Retrieves a user's enrollment for a specific course, including approval status.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The enrollment if found; otherwise, null.</returns>
    public async Task<Enrollment?> GetUserEnrollmentAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching enrollment for user {UserId} in course {CourseId}.",
            userId,
            courseId
        );
        return await _repositoryWrapper
            .Repository<Enrollment>()
            .FindFirstByConditionAsync(
                e => e.IsActive && e.UserId == userId && e.CourseId == courseId,
                cancellationToken
            );
    }

    /// <summary>
    /// Checks if a user has already reviewed a specific course.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the user has reviewed the course; otherwise, false.</returns>
    public async Task<bool> HasUserReviewedAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Checking review existence for user {UserId} on course {CourseId}.",
            userId,
            courseId
        );
        return await _repositoryWrapper.Review.AnyByConditionAsync(
            r => r.IsActive && r.UserId == userId && r.CourseId == courseId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves a course by its unique identifier.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course if found; otherwise, null.</returns>
    public async Task<Course?> GetCourseByIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching course {CourseId}.", courseId);
        return await _repositoryWrapper
            .Repository<Course>()
            .FindFirstByConditionAsync(c => c.IsActive && c.Id == courseId, cancellationToken);
    }

    /// <summary>
    /// Retrieves a review by its unique identifier.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The review if found; otherwise, null.</returns>
    public async Task<Review?> GetReviewByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching review {ReviewId}.", id);
        return await _repositoryWrapper.Review.FindFirstByConditionAsync(
            r => r.IsActive && r.Id == id,
            cancellationToken
        );
    }

    /// <summary>
    /// Adds a new review to the database.
    /// </summary>
    /// <param name="review">The review entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddReviewAsync(Review review, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Adding review for course {CourseId} by user {UserId}.",
            review.CourseId,
            review.UserId
        );
        await _repositoryWrapper.Review.CreateAsync(review, cancellationToken);
    }

    /// <summary>
    /// Updates an existing review in the database.
    /// </summary>
    /// <param name="review">The review entity to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateReviewAsync(
        Review review,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Updating review {ReviewId}.", review.Id);
        _repositoryWrapper.Review.Update(review);
    }

    /// <summary>
    /// Soft deletes a review in the database.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected by the deletion.</returns>
    public async Task<int> DeleteReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Soft-deleting review {ReviewId}.", id);
        return await _repositoryWrapper.Review.SoftDeleteAsync(r => r.Id == id, cancellationToken);
    }

    /// <summary>
    /// Updates the course entity in the database.
    /// </summary>
    /// <param name="course">The course entity to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateCourseAsync(
        Course course,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Updating course {CourseId} overall rating.", course.Id);
        _repositoryWrapper.Repository<Course>().Update(course);
    }
}
