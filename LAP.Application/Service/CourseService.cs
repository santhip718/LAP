using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="ICourseService"/> using <see cref="IRepositoryWrapper"/>.
/// </summary>
public class CourseService : ICourseService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<CourseService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CourseService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public CourseService(IRepositoryWrapper repositoryWrapper, ICustomLogger<CourseService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all courses.</returns>
    public async Task<List<Course>> GetAllCourseAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all courses.");
        return (await _repositoryWrapper.Course.GetAllAsync(cancellationToken)).ToList();
    }

    /// <summary>
    /// Retrieves a course by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching course if found and active; otherwise, <c>null</c>.</returns>
    public async Task<Course?> GetCourseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving course {CourseId}.", id);

        return await _repositoryWrapper.Course.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Adds a new course and returns the created entity.
    /// </summary>
    /// <param name="course">The course entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created course entity.</returns>
    public async Task<Course> AddCourseAsync(
        Course course,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding course {CourseId}.", course.Id);

        return await _repositoryWrapper.Course.AddAsync(course, cancellationToken);
    }

    /// <summary>
    /// Updates an existing course in the repository.
    /// </summary>
    /// <param name="course">The course entity containing the updated property values.</param>
    public void UpdateCourse(Course course)
    {
        _logger.LogDebug("Updating course {CourseId}.", course.Id);
        _repositoryWrapper.Course.Update(course);
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    /// <param name="course">The course entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting course {CourseId}.", id);
        return await _repositoryWrapper.Course.SoftDeleteAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of courses based on various filters.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="categoryId">Optional category identifier to filter by.</param>
    /// <param name="difficultyLevelId">Optional difficulty level identifier to filter by.</param>
    /// <param name="status">Optional status filter (true for active/published, false for draft).</param>
    /// <param name="search">Optional search string to filter by course title.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A tuple containing the list of courses and the total count of courses matching the filters.</returns>
    public async Task<(IEnumerable<Course> Item, int TotalCount)> GetPagedCoursesAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        Guid? difficultyLevelId,
        bool? status,
        string? search,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching paged courses for page {Page} with page size {PageSize}.",
            page,
            pageSize
        );

        IQueryable<Course> query = _repositoryWrapper
            .Course.FindByCondition(c => c.IsActive)
            .Include(c => c.Category)
            .Include(c => c.DifficultyLevel);

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        if (difficultyLevelId.HasValue)
        {
            query = query.Where(c => c.DifficultyLevelId == difficultyLevelId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.IsDrafted == !status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => EF.Functions.Like(c.Title, $"%{search}%"));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Course> item = await query
            .OrderByDescending(c => c.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved {Count} items for paged courses.", item.Count);

        return (item, totalCount);
    }

    /// <summary>
    /// Retrieves a list of recommended courses for a user based on their enrollment history.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="count">The maximum number of recommended courses to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of recommended course entities.</returns>
    public async Task<IEnumerable<Course>> GetRecommendedCourseAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching recommended courses for user {UserId}.", userId);

        List<Guid> enrolledCategoryIds = await _repositoryWrapper
            .Enrollment.FindByCondition(e => e.IsActive && e.UserId == userId)
            .Select(e => e.Course.CategoryId)
            .Distinct()
            .ToListAsync(cancellationToken);

        List<Guid> currentCourseIds = await _repositoryWrapper
            .Enrollment.FindByCondition(e => e.IsActive && e.UserId == userId)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        IQueryable<Course> query = _repositoryWrapper
            .Course.FindByCondition(c =>
                c.IsActive && !c.IsDrafted && !currentCourseIds.Contains(c.Id)
            )
            .Include(c => c.Category)
            .Include(c => c.DifficultyLevel);

        if (enrolledCategoryIds.Any())
        {
            query = query.Where(c => enrolledCategoryIds.Contains(c.CategoryId));
        }

        var result = await query
            .OrderByDescending(c => c.OverallRating)
            .Take(count)
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Retrieved {Count} recommended courses for user {UserId}.",
            result.Count,
            userId
        );

        return result;
    }

    /// <summary>
    /// Retrieves the overview details of a course, including its related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The course entity with loaded navigation properties, or null if not found.</returns>
    public async Task<Course?> GetCourseOverviewAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching overview for course {CourseId}.", id);

        Course? result = await _repositoryWrapper
            .Course.FindByCondition(c => c.IsActive && c.Id == id)
            .Include(c => c.Category)
            .Include(c => c.SubCategory)
            .Include(c => c.DifficultyLevel)
            .Include(c => c.Assessment)
            .Include(c => c.Enrollments)
            .Include(c => c.CreatedByUser)
                .ThenInclude(u => u.Person)
            .Include(c => c.Topics.Where(t => t.IsActive).OrderBy(t => t.SequenceOrder))
                .ThenInclude(t => t.Contents.Where(cc => cc.IsActive).OrderBy(cc => cc.SequenceOrder))
            .FirstOrDefaultAsync(cancellationToken);

        _logger.LogDebug("Overview for course {CourseId} found: {Found}.", id, result != null);

        return result;
    }

    /// <summary>
    /// Retrieves a course with the user's progress for each content item.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The course entity with progress information, or null if not found.</returns>
    public async Task<Course?> GetCourseWithProgressAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching course {CourseId} with progress for user {UserId}.", id, userId);

        Course? result = await _repositoryWrapper
            .Course.FindByCondition(c => c.IsActive && c.Id == id)
            .Include(c => c.Topics.Where(t => t.IsActive).OrderBy(t => t.SequenceOrder))
                .ThenInclude(t => t.Contents.Where(cc => cc.IsActive).OrderBy(cc => cc.SequenceOrder))
                    .ThenInclude(cc => cc.ContentType)
            .Include(c => c.Topics.Where(t => t.IsActive).OrderBy(t => t.SequenceOrder))
                .ThenInclude(t => t.Contents.Where(cc => cc.IsActive).OrderBy(cc => cc.SequenceOrder))
                    .ThenInclude(cc =>
                        cc.UserCourseProgresses.Where(p => p.Enrollment.UserId == userId)
                    )
            .FirstOrDefaultAsync(cancellationToken);

        _logger.LogDebug("Course {CourseId} with progress found: {Found}.", id, result != null);

        return result;
    }

    /// <summary>
    /// Retrieves the enrollment record for a specific course and user.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The enrollment record if found; otherwise, null.</returns>
    public async Task<Enrollment?> GetEnrollmentAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching enrollment for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );

        Enrollment? result = await _repositoryWrapper.Enrollment.FindFirstByConditionAsync(
            e => e.IsActive && e.CourseId == courseId && e.UserId == userId,
            cancellationToken
        );

        _logger.LogDebug(
            "Enrollment for course {CourseId} found: {Found}.",
            courseId,
            result != null
        );

        return result;
    }

    /// <summary>
    /// Retrieves the assessment history for a user in a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A tuple containing the collection of assessment history items and the total count.</returns>
    public async Task<(
        IEnumerable<AssessmentHistory> Item,
        int TotalCount
    )> GetAssessmentHistoryAsync(
        Guid courseId,
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching assessment history for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );

        IQueryable<AssessmentHistory> query = _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive && ah.Assessment.CourseId == courseId && ah.UserId == userId
            )
            .Include(ah => ah.Assessment)
            .Include(ah => ah.TierAwarded);

        int totalCount = await query.CountAsync(cancellationToken);

        List<AssessmentHistory> item = await query
            .OrderByDescending(ah => ah.StartedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Retrieved {Count} assessment history items for course {CourseId}.",
            item.Count,
            courseId
        );

        return (item, totalCount);
    }

    /// <summary>
    /// Retrieves categories that have at least one active course.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of RefTerm entities representing active categories.</returns>
    public async Task<List<RefTerm>> GetActiveCategoryAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching categories with active courses.");

        List<Guid> categoryIds = await _repositoryWrapper
            .Course.FindByCondition(c => c.IsActive)
            .Select(c => c.CategoryId)
            .Distinct()
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Found {Count} unique category IDs with active courses.",
            categoryIds.Count
        );

        List<RefTerm> categories = await _repositoryWrapper
            .Repository<RefTerm>()
            .FindByCondition(r => r.IsActive && categoryIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Found {Count} active categories with courses.",
            categories.Count
        );

        return categories;
    }

    /// <summary>
    /// Gets the total number of active course contents for a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The total number of active content items.</returns>
    public async Task<int> GetTotalCourseContentCountAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Counting total contents for course {CourseId}.", courseId);

        int count = await _repositoryWrapper
            .Repository<CourseContent>()
            .CountByConditionNoTrackingAsync(
                cc => cc.IsActive && cc.MetaTopic.CourseId == courseId,
                cancellationToken
            );

        _logger.LogDebug("Course {CourseId} has {Count} total contents.", courseId, count);

        return count;
    }

    /// <summary>
    /// Gets the number of completed course contents for a user in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The count of completed contents for the user in the course.</returns>
    public async Task<int> GetCompletedCourseContentCountAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Counting completed contents for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );

        int count = await _repositoryWrapper
            .Repository<UserCourseProgress>()
            .CountByConditionNoTrackingAsync(
                ucp =>
                    ucp.IsActive
                    && ucp.IsCompleted
                    && ucp.Enrollment.CourseId == courseId
                    && ucp.Enrollment.UserId == userId,
                cancellationToken
            );

        _logger.LogDebug(
            "User {UserId} has completed {Count} contents in course {CourseId}.",
            userId,
            count,
            courseId
        );

        return count;
    }

    /// <summary>
    /// Checks whether a course with the given title already exists in the specified category.
    /// </summary>
    /// <param name="title">The course title to check.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="excludeCourseId">Optional course ID to exclude from the check (e.g., the course being updated).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if a course with the same title exists in the category.</returns>
    public async Task<bool> IsCourseNameExistAsync(
        string title,
        Guid categoryId,
        Guid? excludeCourseId = null,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Checking if course '{Title}' exists in category {CategoryId} (excluding course {ExcludeId}).",
            title,
            categoryId,
            excludeCourseId
        );

        return await _repositoryWrapper.Course.AnyByConditionAsync(
            c => c.IsActive
                && c.Title == title
                && c.CategoryId == categoryId
                && (!excludeCourseId.HasValue || c.Id != excludeCourseId.Value),
            cancellationToken
        );
    }

    /// <summary>
    /// Requests enrollment for a user in a specific course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The enrollment if created successfully; otherwise, null.</returns>
    public async Task<Enrollment?> RequestEnrollmentAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Requesting enrollment for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );

        // Check if course exists
        Course? course = await _repositoryWrapper.Course.FindFirstByConditionAsync(
            c => c.IsActive && c.Id == courseId,
            cancellationToken
        );

        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found for enrollment request.", courseId);
            return null;
        }

        // Check if already enrolled
        Enrollment? existingEnrollment =
            await _repositoryWrapper.Enrollment.FindFirstByConditionAsNoTrackingAsync(
                e => e.IsActive && e.CourseId == courseId && e.UserId == userId,
                cancellationToken
            );
        if (existingEnrollment != null)
        {
            _logger.LogError(
                "User {UserId} already enrolled in course {CourseId}.",
                userId,
                courseId
            );
            throw new LAP.Shared.Exceptions.ConflictException(
                "Already enrolled",
                "You are already enrolled in this course."
            );
        }

        Enrollment enrollment = new Enrollment
        {
            CourseId = courseId,
            UserId = userId,
            EnrolledOn = DateTime.UtcNow,
            EnrollmentStatus = true, // Default to active for now, or follow business logic if approval needed
            ProgressPercentage = 0,
        };

        await _repositoryWrapper.Enrollment.CreateAsync(enrollment, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Enrollment {EnrollmentId} created for course {CourseId} and user {UserId}.",
            enrollment.Id,
            courseId,
            userId
        );

        return enrollment;
    }
}
