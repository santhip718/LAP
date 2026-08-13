using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for assessment-related operations.
/// </summary>
public interface IAssessmentService
{
    /// <summary>
    /// Retrieves meta topics associated with a course.
    /// </summary>
    Task<List<CourseMetaTopic>> GetMetaTopicByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves an assessment along with its associated questions asynchronously.
    /// </summary>
    /// <param name="assessmentId">The unique identifier of the assessment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the assessment if found; otherwise, null.</returns>
    Task<Assessment?> GetAssessmentWithQuestionsAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves question types.
    /// </summary>
    Task<List<RefTerm>> GetQuestionTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an assessment by its unique identifier.
    /// </summary>
    /// <param name="assessmentId">The unique identifier of the assessment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The assessment if found; otherwise, null.</returns>
    Task<Assessment?> GetAssessmentByIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves questions for a specific assessment.
    /// </summary>
    Task<List<Question>> GetQuestionByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new assessment entity.
    /// </summary>
    Task AddAssessmentAsync(Assessment assessment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing assessment.
    /// </summary>
    Task UpdateAssessmentAsync(
        Assessment assessment,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new question entity.
    /// </summary>
    Task AddQuestionAsync(Question question, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all assessments.
    /// </summary>
    Task<List<Assessment>> GetAllAssessmentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of assessments.
    /// </summary>
    Task<(List<Assessment> Items, int TotalCount)> GetAllAssessmentPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if an active assessment already exists for the specified course.
    /// </summary>
    Task<bool> ActiveAssessmentExistsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Counts the active questions for a specific assessment.
    /// </summary>
    Task<int> CountActiveQuestionByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes an assessment by its identifier.
    /// </summary>
    Task<int> DeleteAssessmentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a question by its unique identifier.
    /// </summary>
    Task<Question?> GetQuestionByIdAsync(
        Guid questionId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing question.
    /// </summary>
    Task UpdateQuestionAsync(Question question, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a question by its identifier.
    /// </summary>
    Task<int> DeleteQuestionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves assessment overview by course identifier.
    /// </summary>
    Task<List<Assessment>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a user is currently enrolled in a specific course asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the user is enrolled; otherwise, false.</returns>
    Task<bool> IsUserEnrolledAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a course exists by its identifier.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the course exists; otherwise, false.</returns>
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all assessment attempt records for a specific user and assessment asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="assessmentId">The unique identifier of the assessment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of assessment history records.</returns>
    Task<IEnumerable<AssessmentHistory>> GetUserAssessmentAttemptAsync(
        Guid userId,
        Guid assessmentId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a new assessment history record asynchronously.
    /// </summary>
    /// <param name="history">The assessment history entity to persist.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created assessment history record.</returns>
    Task<AssessmentHistory> AddAssessmentHistoryAsync(
        AssessmentHistory history,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Persists a range of assessment answers asynchronously.
    /// </summary>
    /// <param name="answer">A collection of assessment answer entities to persist.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAssessmentAnswerRangeAsync(
        IEnumerable<AssessmentAnswer> answer,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all assessment history records for a user within a specific course asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of assessment history records.</returns>
    Task<IEnumerable<AssessmentHistory>> GetUserCourseAssessmentHistoriesAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all history records for a specific assessment and user asynchronously.
    /// </summary>
    /// <param name="assessmentId">The unique identifier of the assessment.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of assessment history records.</returns>
    Task<IEnumerable<AssessmentHistory>> GetAllAssessmentHistoriesAsync(
        Guid assessmentId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a paginated list of assessment history records for a user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated items and the total count of records.</returns>
    Task<(IEnumerable<AssessmentHistory> Item, int TotalCount)> GetPagedAssessmentHistoryAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the appropriate tier reference term for a given score asynchronously.
    /// </summary>
    /// <param name="score">The score percentage to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the tier reference term if found; otherwise, null.</returns>
    Task<RefTerm?> GetTierByScoreAsync(
        decimal score,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing assessment history record.
    /// </summary>
    /// <param name="history">The assessment history entity with updated values.</param>
    void UpdateAssessmentHistory(AssessmentHistory history);

    /// <summary>
    /// Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user record.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    void UpdateUser(User user);

    /// <summary>
    /// Retrieves all completed assessment histories for a user across all courses.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of completed assessment history records.</returns>
    Task<IEnumerable<AssessmentHistory>> GetUserAllCompletedAssessmentHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all active tier reference terms asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of tier reference terms.</returns>
    Task<IEnumerable<RefTerm>> GetTierAsync(CancellationToken cancellationToken = default);
}
