using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="IEnrollmentService"/> using <see cref="IRepositoryWrapper"/>.
/// </summary>
public class EnrollmentService : IEnrollmentService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<EnrollmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public EnrollmentService(
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<EnrollmentService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all enrollments.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all enrollments.</returns>
    public async Task<List<Enrollment>> GetAllEnrollmentAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving all enrollments.");
        return (await _repositoryWrapper.Enrollment.GetAllAsync(cancellationToken)).ToList();
    }

    /// <summary>
    /// Checks whether a user is already enrolled in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if the user is already enrolled; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsUserEnrolledAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Checking enrollment for user {UserId} in course {CourseId}.",
            userId,
            courseId
        );

        return await _repositoryWrapper.Enrollment.AnyByConditionNoTrackingAsync(
            e => e.CourseId == courseId && e.UserId == userId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves a filtered list of enrollments by course name, category, and user.
    /// </summary>
    /// <param name="courseName">An optional course name filter to narrow results.</param>
    /// <param name="categoryId">An optional category identifier filter to narrow results.</param>
    /// <param name="userId">An optional user identifier filter to narrow results.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of enrollments matching the specified filter criteria.</returns>
    public async Task<List<Enrollment>> GetEnrollmentAsync(
        string? courseName,
        Guid? categoryId,
        Guid? userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Retrieving enrollments for course name {CourseName}, category {CategoryId}, and user {UserId}.",
            courseName,
            categoryId,
            userId
        );

        var query = _repositoryWrapper
            .Enrollment.GetByConditionNoTracking(e => true)
            .Include(e => e.Course)
                .ThenInclude(c => c.Category)
            .Include(e => e.User)
                .ThenInclude(u => u.Person)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            query = query.Where(e => e.Course.Title.Contains(courseName));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.Course.CategoryId == categoryId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(e => e.UserId == userId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an enrollment by its unique identifier with full details.
    /// </summary>
    /// <param name="id">The unique identifier of the enrollment to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching enrollment with full details if found; otherwise, <c>null</c>.</returns>
    public async Task<Enrollment?> GetEnrollmentByIdWithDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving enrollment {EnrollmentId}.", id);

        return await _repositoryWrapper
            .Enrollment.GetByConditionNoTracking(e => e.Id == id)
            .Include(e => e.Course)
                .ThenInclude(c => c.Category)
            .Include(e => e.User)
                .ThenInclude(u => u.Person)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing enrollment in the repository.
    /// </summary>
    /// <param name="enrollment">The enrollment entity containing the updated property values.</param>
    public void UpdateEnrollment(Enrollment enrollment)
    {
        _logger.LogDebug("Updating enrollment {EnrollmentId}.", enrollment.Id);

        _repositoryWrapper.Enrollment.Update(enrollment);
    }

    /// <summary>
    /// Adds a new enrollment and returns the created entity.
    /// </summary>
    /// <param name="enrollment">The enrollment entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created enrollment entity.</returns>
    public async Task<Enrollment> AddEnrollmentAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Adding enrollment for user {UserId} in course {CourseId}.",
            enrollment.UserId,
            enrollment.CourseId
        );

        return await _repositoryWrapper.Enrollment.AddAsync(enrollment, cancellationToken);
    }
}
