using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for course-related operations.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    Task<List<Course>> GetAllCourseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a course by its unique identifier.
    /// </summary>
    Task<Course?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new course and returns the created entity.
    /// </summary>
    Task<Course> AddCourseAsync(Course course, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    void UpdateCourse(Course course);

    /// <summary>
    /// Deletes a course by its identifier.
    /// </summary>
    Task<int> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated and filtered list of courses.
    /// </summary>
    Task<(IEnumerable<Course> Item, int TotalCount)> GetPagedCoursesAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        Guid? difficultyLevelId,
        bool? status,
        string? search,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves recommended courses for a user.
    /// </summary>
    Task<IEnumerable<Course>> GetRecommendedCourseAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a detailed overview of a course.
    /// </summary>
    Task<Course?> GetCourseOverviewAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves course topics and contents with user-specific progress.
    /// </summary>
    Task<Course?> GetCourseWithProgressAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a user's enrollment for a course.
    /// </summary>
    Task<Enrollment?> GetEnrollmentAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves assessment history for a specific course and user.
    /// </summary>
    Task<(IEnumerable<AssessmentHistory> Item, int TotalCount)> GetAssessmentHistoryAsync(
        Guid courseId,
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks whether a course with the given title already exists in the specified category.
    /// </summary>
    Task<bool> IsCourseNameExistAsync(
        string title,
        Guid categoryId,
        Guid? excludeCourseId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Requests enrollment for a user in a specific course.
    /// </summary>
    Task<Enrollment?> RequestEnrollmentAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves categories that have at least one active course.
    /// </summary>
    Task<List<RefTerm>> GetActiveCategoryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of active course contents for a course.
    /// </summary>
    Task<int> GetTotalCourseContentCountAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the number of completed course contents for a user in a course.
    /// </summary>
    Task<int> GetCompletedCourseContentCountAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
