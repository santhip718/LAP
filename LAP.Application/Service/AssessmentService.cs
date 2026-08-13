using LAP.Application.Constant;
using LAP.Application.Helper;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Service for managing assessment-related operations.
/// </summary>
public class AssessmentService : IAssessmentService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<AssessmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper.</param>
    /// <param name="logger">The custom logger.</param>
    public AssessmentService(
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<AssessmentService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves meta topics associated with a course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of meta topics for the specified course.</returns>
    public async Task<List<CourseMetaTopic>> GetMetaTopicByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving meta topics for course {CourseId}", courseId);
        return await _repositoryWrapper
            .Repository<CourseMetaTopic>()
            .FindByCondition(x => x.IsActive && x.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves question types.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of question types.</returns>
    public async Task<List<RefTerm>> GetQuestionTypeAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving question types");
        return await _repositoryWrapper
            .Repository<RefTerm>()
            .FindByCondition(x =>
                x.IsActive && x.RefSet.Name == CommonConstants.QUESTION_TYPE_REF_SET_NAME
            )
            .Include(x => x.RefSet)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an assessment with its associated questions.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The assessment if found; otherwise, null.</returns>
    public async Task<Assessment?> GetAssessmentWithQuestionsAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching assessment {AssessmentId} with questions.", assessmentId);
        return await _repositoryWrapper
            .Assessment.FindByCondition(a => a.IsActive && a.Id == assessmentId)
            .Include(a => a.Questions.Where(q => q.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an assessment by its identifier.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The assessment if found; otherwise, null.</returns>
    public async Task<Assessment?> GetAssessmentByIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching assessment {AssessmentId}.", assessmentId);
        return await _repositoryWrapper.Assessment.FindFirstByConditionAsync(
            a => a.IsActive && a.Id == assessmentId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves questions by assessment identifier.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of questions for the specified assessment.</returns>
    public async Task<List<Question>> GetQuestionByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving questions for assessment {AssessmentId}.", assessmentId);
        return await _repositoryWrapper
            .Repository<Question>()
            .FindByCondition(x => x.IsActive && x.AssessmentId == assessmentId)
            .Include(x => x.QuestionType)
            .ToListAsync(cancellationToken);
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
        return await _repositoryWrapper.Enrollment.AnyByConditionAsync(
            e => e.IsActive && e.UserId == userId && e.CourseId == courseId,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves the list of assessment attempts for a specific user and assessment.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of assessment history records.</returns>
    public async Task<IEnumerable<AssessmentHistory>> GetUserAssessmentAttemptAsync(
        Guid userId,
        Guid assessmentId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching assessment attempts for user {UserId} on assessment {AssessmentId}.",
            userId,
            assessmentId
        );
        return await _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive
                && ah.UserId == userId
                && ah.AssessmentId == assessmentId
                && ah.CompletedOn != null
            )
            .OrderByDescending(ah => ah.CompletedOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new assessment entity.
    /// </summary>
    /// <param name="assessment">The assessment entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddAssessmentAsync(
        Assessment assessment,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding assessment for course {CourseId}.", assessment.CourseId);
        await _repositoryWrapper.Assessment.CreateAsync(assessment, cancellationToken);
    }

    /// <summary>
    /// Updates an existing assessment.
    /// </summary>
    /// <param name="assessment">The assessment entity containing the updated property values.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task UpdateAssessmentAsync(
        Assessment assessment,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Updating assessment {AssessmentId}.", assessment.Id);
        _repositoryWrapper.Assessment.Update(assessment);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new question entity.
    /// </summary>
    /// <param name="question">The question entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddQuestionAsync(
        Question question,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding question for assessment {AssessmentId}.", question.AssessmentId);
        await _repositoryWrapper.Repository<Question>().CreateAsync(question, cancellationToken);
    }

    /// <summary>
    /// Saves pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes to the database.");
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all assessments.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of all assessments.</returns>
    public async Task<List<Assessment>> GetAllAssessmentAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving all assessments.");
        return await _repositoryWrapper
            .Assessment.FindByCondition(x => x.IsActive)
            .Include(x => x.Course)
                .ThenInclude(c => c.Category)
            .Include(x => x.Course)
                .ThenInclude(c => c.DifficultyLevel)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of assessments.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A tuple containing the assessments for the current page and the total count.</returns>
    public async Task<(List<Assessment> Items, int TotalCount)> GetAllAssessmentPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Retrieving paginated assessments (page {PageNumber}, size {PageSize}).",
            pageNumber,
            pageSize
        );

        IQueryable<Assessment> query = _repositoryWrapper
            .Assessment.FindByCondition(x => x.IsActive)
            .Include(x => x.Course)
                .ThenInclude(c => c.Category)
            .Include(x => x.Course)
                .ThenInclude(c => c.DifficultyLevel);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Assessment> items = await query
            .OrderByDescending(x => x.DateCreated)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Checks if an active assessment already exists for the specified course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if an active assessment exists for the course; otherwise, <c>false</c>.</returns>
    public async Task<bool> ActiveAssessmentExistsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking for active assessment for course {CourseId}.", courseId);
        return await _repositoryWrapper.Assessment.AnyByConditionAsync(
            a => a.IsActive && a.CourseId == courseId,
            cancellationToken
        );
    }

    /// <summary>
    /// Counts the active questions for a specific assessment.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of active questions for the assessment.</returns>
    public async Task<int> CountActiveQuestionByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Counting active questions for assessment {AssessmentId}.", assessmentId);
        return await _repositoryWrapper
            .Repository<Question>()
            .CountByConditionNoTrackingAsync(
                q => q.IsActive && q.AssessmentId == assessmentId,
                cancellationToken
            );
    }

    /// <summary>
    /// Deletes an assessment.
    /// </summary>
    /// <param name="assessment">The assessment entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task<int> DeleteAssessmentAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Deleting assessment {AssessmentId}.", id);
        return await _repositoryWrapper.Assessment.SoftDeleteAsync(
            a => a.Id == id,
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves a question by its unique identifier.
    /// </summary>
    /// <param name="questionId">The unique identifier of the question.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching question if found; otherwise, <c>null</c>.</returns>
    public async Task<Question?> GetQuestionByIdAsync(
        Guid questionId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving question {QuestionId}.", questionId);
        return await _repositoryWrapper
            .Repository<Question>()
            .FindFirstByConditionAsync(x => x.IsActive && x.Id == questionId, cancellationToken);
    }

    /// <summary>
    /// Updates an existing question.
    /// </summary>
    /// <param name="question">The question entity containing the updated property values.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task UpdateQuestionAsync(
        Question question,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Updating question {QuestionId}.", question.Id);
        _repositoryWrapper.Repository<Question>().Update(question);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a question.
    /// </summary>
    /// <param name="question">The question entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task<int> DeleteQuestionAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Deleting question {QuestionId}.", id);
        return await _repositoryWrapper
            .Repository<Question>()
            .SoftDeleteAsync(q => q.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves assessment overview by course identifier.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of assessments for the specified course.</returns>
    public async Task<List<Assessment>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving assessments for course {CourseId}.", courseId);
        return await _repositoryWrapper
            .Assessment.FindByCondition(x => x.IsActive && x.CourseId == courseId)
            .Include(x => x.Course)
                .ThenInclude(c => c.Category)
            .Include(x => x.Course)
                .ThenInclude(c => c.DifficultyLevel)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new assessment history record.
    /// </summary>
    /// <param name="history">The assessment history record to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The added assessment history record.</returns>
    public async Task<AssessmentHistory> AddAssessmentHistoryAsync(
        AssessmentHistory history,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding assessment history for user {UserId}.", history.UserId);
        await _repositoryWrapper.AssessmentHistory.CreateAsync(history, cancellationToken);
        return history;
    }

    /// <summary>
    /// Adds a range of assessment answers.
    /// </summary>
    /// <param name="answer">The collection of assessment answers to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAssessmentAnswerRangeAsync(
        IEnumerable<AssessmentAnswer> answer,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding assessment answers range.");
        await _repositoryWrapper.AssessmentAnswer.CreateRangeAsync(
            answer.ToList(),
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves the assessment histories for a user in a specific course.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of assessment history records with associated assessments.</returns>
    public async Task<IEnumerable<AssessmentHistory>> GetUserCourseAssessmentHistoriesAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching assessment histories for user {UserId} in course {CourseId}.",
            userId,
            courseId
        );
        return await _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive
                && ah.Assessment.CourseId == courseId
                && ah.UserId == userId
                && ah.CompletedOn != null
            )
            .Include(ah => ah.Assessment)
            .OrderByDescending(ah => ah.CompletedOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a course exists by its identifier.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if the course exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> CourseExistsAsync(
        Guid courseId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking existence of course {CourseId}.", courseId);
        return await _repositoryWrapper
            .Repository<Course>()
            .AnyByConditionAsync(x => x.IsActive && x.Id == courseId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all assessment histories for a specific assessment and user.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of assessment history records.</returns>
    public async Task<IEnumerable<AssessmentHistory>> GetAllAssessmentHistoriesAsync(
        Guid assessmentId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching all assessment histories for assessment {AssessmentId} and user {UserId}.",
            assessmentId,
            userId
        );
        return await _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive
                && ah.AssessmentId == assessmentId
                && ah.UserId == userId
                && ah.CompletedOn != null
            )
            .Include(ah => ah.Assessment)
            .OrderBy(ah => ah.CompletedOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paged collection of assessment history records for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A tuple containing the paged items and the total count.</returns>
    public async Task<(
        IEnumerable<AssessmentHistory> Item,
        int TotalCount
    )> GetPagedAssessmentHistoryAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching paged assessment history for user {UserId} (page {Page}, size {Size}).",
            userId,
            pageNumber,
            pageSize
        );

        IQueryable<AssessmentHistory> query = _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive && ah.UserId == userId && ah.CompletedOn != null
            )
            .Include(ah => ah.Assessment)
                .ThenInclude(a => a.Course);

        int totalCount = await query.CountAsync(cancellationToken);

        List<AssessmentHistory> item = await query
            .OrderByDescending(ah => ah.CompletedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (item, totalCount);
    }

    /// <summary>
    /// Retrieves a tier reference term based on the given score.
    /// </summary>
    /// <param name="score">The score percentage.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The reference term if found; otherwise, null.</returns>
    public async Task<RefTerm?> GetTierByScoreAsync(
        decimal score,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching tier for score {Score}.", score);
        string tierName = TierCalculationHelper.GetTierName(score);

        RefSet? tierRefSet = await _repositoryWrapper
            .Repository<RefSet>()
            .FindFirstByConditionAsync(
                rs => rs.IsActive && rs.Name == CommonConstants.TIER_REF_SET_NAME,
                cancellationToken
            );

        if (tierRefSet is null)
        {
            _logger.LogError(
                "'{TierRefSetName}' reference set not found.",
                CommonConstants.TIER_REF_SET_NAME
            );
            return null;
        }

        return await _repositoryWrapper
            .Repository<RefTerm>()
            .FindFirstByConditionAsync(
                rt => rt.IsActive && rt.RefSetId == tierRefSet.Id && rt.Name == tierName,
                cancellationToken
            );
    }

    /// <summary>
    /// Updates an existing assessment history record.
    /// </summary>
    /// <param name="history">The assessment history record to update.</param>
    public void UpdateAssessmentHistory(AssessmentHistory history)
    {
        _logger.LogDebug("Updating assessment history {HistoryId}.", history.Id);
        _repositoryWrapper.AssessmentHistory.Update(history);
    }

    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching user {UserId}.", userId);
        return await _repositoryWrapper.User.FindFirstByConditionAsync(
            u => u.IsActive && u.Id == userId,
            cancellationToken
        );
    }

    /// <summary>
    /// Updates an existing user record.
    /// </summary>
    /// <param name="user">The user record to update.</param>
    public void UpdateUser(User user)
    {
        _logger.LogDebug("Updating user {UserId}.", user.Id);
        _repositoryWrapper.User.Update(user);
    }

    /// <summary>
    /// Retrieves all completed assessment histories for a user across all courses.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of completed assessment history records.</returns>
    public async Task<IEnumerable<AssessmentHistory>> GetUserAllCompletedAssessmentHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching all completed assessment histories for user {UserId}.", userId);
        return await _repositoryWrapper
            .AssessmentHistory.FindByCondition(ah =>
                ah.IsActive && ah.UserId == userId && ah.CompletedOn != null
            )
            .Include(ah => ah.Assessment)
            .OrderByDescending(ah => ah.CompletedOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all active tier reference terms.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of tier reference terms.</returns>
    public async Task<IEnumerable<RefTerm>> GetTierAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching all active tier reference terms.");
        RefSet tierRefSet = (
            await _repositoryWrapper
                .Repository<RefSet>()
                .FindFirstByConditionAsync(
                    rs => rs.IsActive && rs.Name == CommonConstants.TIER_REF_SET_NAME,
                    cancellationToken
                )
        )!;

        return await _repositoryWrapper
            .Repository<RefTerm>()
            .FindByCondition(rt => rt.IsActive && rt.RefSetId == tierRefSet.Id)
            .ToListAsync(cancellationToken);
    }
}
