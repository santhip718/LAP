using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Helper;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using AssessmentAnswerEntity = LAP.Domain.Entity.AssessmentAnswer;
using AssessmentEntity = LAP.Domain.Entity.Assessment;
using AssessmentHistoryEntity = LAP.Domain.Entity.AssessmentHistory;
using CourseMetaTopicEntity = LAP.Domain.Entity.CourseMetaTopic;
using QuestionEntity = LAP.Domain.Entity.Question;
using RefTermEntity = LAP.Domain.Entity.RefTerm;

namespace LAP.Application.Feature.Assessment.Command;

/// <summary>
/// Command to submit assessment answers for evaluation.
/// </summary>
/// <param name="AssessmentId">The identifier of the assessment being submitted.</param>
/// <param name="Dto">The submission payload containing user answers.</param>
public record SubmitAssessmentCommand(Guid AssessmentId, AssessmentSubmitRequestDto Dto)
    : IRequest<SubmitAssessmentResponseDto>;

/// <summary>
/// Validates the <see cref="SubmitAssessmentCommand"/> request data.
/// </summary>
public class SubmitAssessmentValidator : AbstractValidator<SubmitAssessmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitAssessmentValidator"/> class.
    /// </summary>
    public SubmitAssessmentValidator()
    {
        RuleFor(x => x.AssessmentId).NotEmpty().WithMessage("Assessment identifier is required");

        RuleFor(x => x.Dto).NotNull().WithMessage("Submission details are required");

        When(
            x => x.Dto is not null,
            () =>
            {
                RuleFor(x => x.Dto.UserId).NotEmpty().WithMessage("User identifier is required");

                RuleFor(x => x.Dto.StartedOn)
                    .NotEmpty()
                    .WithMessage("Started on date is required")
                    .LessThan(DateTime.UtcNow)
                    .WithMessage("Started on date must be in the past");

                RuleFor(x => x.Dto.Answer)
                    .NotNull()
                    .WithMessage("Answers collection is required");

                When(
                    x => x.Dto.Answer is not null,
                    () =>
                    {
                        RuleForEach(x => x.Dto.Answer)
                            .ChildRules(answer =>
                            {
                                answer
                                    .RuleFor(a => a.QuestionId)
                                    .NotEmpty()
                                    .WithMessage("Question identifier is required");
                            });
                    }
                );
            }
        );
    }
}

/// <summary>
/// Handles the submission of assessment answers, calculates scores, and records the result.
/// Refactored to contain exactly three methods.
/// </summary>
public class SubmitAssessmentHandler
    : IRequestHandler<SubmitAssessmentCommand, SubmitAssessmentResponseDto>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<SubmitAssessmentHandler> _logger;
    private readonly IRequestContext _requestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitAssessmentHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="requestContext">The request context.</param>
    public SubmitAssessmentHandler(
        IAssessmentService assessmentService,
        ITransactionService transactionService,
        ICustomLogger<SubmitAssessmentHandler> logger,
        IRequestContext requestContext
    )
    {
        _assessmentService = assessmentService;
        _transactionService = transactionService;
        _logger = logger;
        _requestContext = requestContext;
    }

    /// <summary>
    /// Processes the assessment submission, validates constraints, calculates scores, and persists results.
    /// </summary>
    /// <param name="request">The submit assessment command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The assessment submission result.</returns>
    public async Task<SubmitAssessmentResponseDto> Handle(
        SubmitAssessmentCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_requestContext.UserId is null)
        {
            throw new UnauthorizedException("User not authenticated", "User is not authenticated.");
        }

        Guid authenticatedUserId = _requestContext.UserId.Value;
        _logger.LogInfo(
            "Started assessment submission for assessment {AssessmentId} by user {UserId}.",
            request.AssessmentId,
            authenticatedUserId
        );

        // Read phase: validate and prepare outside transaction
        var (assessment, questionList, submittedAnswer) = await ValidateAndPrepareAsync(
            request.AssessmentId,
            authenticatedUserId,
            request.Dto,
            cancellationToken
        );

        // Read phase: compute all data needed for writes and response
        ComputedSubmissionData data = await ComputeSubmissionDataAsync(
            assessment,
            questionList,
            submittedAnswer,
            authenticatedUserId,
            request.Dto.StartedOn,
            cancellationToken
        );

        // Compute answer review data
        List<SubmitAnswerReviewDto> answerReview = questionList
            .Select(q =>
            {
                AssessmentAnswerEntity? answer = data.AnswerEntityList.FirstOrDefault(a =>
                    a.QuestionId == q.Id
                );
                return new SubmitAnswerReviewDto
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    SelectedAnswer = answer?.SelectedAnswer,
                    IsCorrect = answer?.IsCorrect ?? false,
                    ObtainedScore = answer is not null ? (int)answer.ObtainedScore : 0,
                };
            })
            .ToList();

        // Compute weak topics
        List<WeakTopicDto> weakTopic = await ComputeWeakTopicsAsync(
            assessment.CourseId,
            questionList,
            data.AnswerEntityList,
            cancellationToken
        );

        // Write phase: only data modification inside transaction
        SubmitAssessmentResponseDto result = await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                await _assessmentService.AddAssessmentHistoryAsync(data.History, cancellationToken);

                _logger.LogInfo(
                    "Created assessment history entity. HistoryId: {HistoryId}, AssessmentId: {AssessmentId}, UserId: {UserId}",
                    data.History.Id,
                    assessment.Id,
                    authenticatedUserId
                );

                await _assessmentService.AddAssessmentAnswerRangeAsync(
                    data.AnswerEntityList,
                    cancellationToken
                );

                _logger.LogInfo(
                    "Successfully added {AnswerCount} assessment answers. HistoryId: {HistoryId}",
                    data.AnswerEntityList.Count,
                    data.History.Id
                );

                data.History.Score = data.Score;
                data.History.WeightedScore = data.WeightedScore;

                await _transactionService.SaveChangesAsync(cancellationToken);

                _logger.LogInfo(
                    "First SaveChangesAsync completed. HistoryId: {HistoryId}, AssessmentId: {AssessmentId}, UserId: {UserId}",
                    data.History.Id,
                    assessment.Id,
                    authenticatedUserId
                );

                data.History.TierAwardedId = data.Tier.Id;
                _assessmentService.UpdateAssessmentHistory(data.History);

                if (data.User != null)
                {
                    IEnumerable<AssessmentHistoryEntity> completedHistory =
                        await _assessmentService.GetUserAllCompletedAssessmentHistoryAsync(
                            authenticatedUserId,
                            cancellationToken
                        );
                    IEnumerable<RefTermEntity> tier = await _assessmentService.GetTierAsync(
                        cancellationToken
                    );

                    decimal averageWeightedScore = completedHistory.Any()
                        ? Math.Round(completedHistory.Average(h => h.WeightedScore), 2)
                        : 0;

                    Guid overallTierId = TierCalculationHelper.CalculateOverallTierId(
                        completedHistory,
                        tier
                    );

                    data.User.OverallWeightedScore = averageWeightedScore;
                    data.User.CurrentTierId = overallTierId;
                    data.User.OverallScore = data.CourseMasteryScore;
                    _assessmentService.UpdateUser(data.User);
                }

                await _transactionService.SaveChangesAsync(cancellationToken);

                _logger.LogInfo(
                    "Second SaveChangesAsync completed. HistoryId: {HistoryId}, TierId: {TierId}, CourseMasteryScore: {CourseMasteryScore}",
                    data.History.Id,
                    data.Tier.Id,
                    data.CourseMasteryScore
                );

                _logger.LogInfo(
                    "Assessment {AssessmentId} submitted by user {UserId}. Score: {Score}/{TotalWeight}, Weighted: {WeightedScore}%, CourseMastery: {CourseMastery}%.",
                    assessment.Id,
                    authenticatedUserId,
                    data.Score,
                    data.TotalWeight,
                    data.WeightedScore,
                    data.CourseMasteryScore
                );

                return new SubmitAssessmentResponseDto
                {
                    AssessmentHistoryId = data.History.Id,
                    AssessmentId = assessment.Id,
                    CourseId = assessment.CourseId,
                    Status = "Completed",
                    StartedOn = request.Dto.StartedOn,
                    CompletedOn = data.UtcNow,
                    DurationTakenMinutes = data.DurationTakenMinutes,
                    TotalQuestion = data.TotalQuestionCount,
                    CorrectAnswer = data.CorrectCount,
                    Score = data.Score,
                    WeightedScore = data.WeightedScore,
                    CourseMasteryScore = data.CourseMasteryScore,
                    Passed = data.Passed,
                    TierAwarded = data.Tier.Name,
                    WeakTopic = weakTopic,
                    Answers = answerReview,
                };
            },
            cancellationToken
        );

        _logger.LogInfo(
            "Completed assessment submission for assessment {AssessmentId} by user {UserId}.",
            request.AssessmentId,
            authenticatedUserId
        );

        return result;
    }

    /// <summary>
    /// Validates that the assessment exists, the user is enrolled, attempts are within limit,
    /// and all submitted question IDs are valid. Returns the assessment, question list, and submitted answer mapping.
    /// </summary>
    private async Task<(
        AssessmentEntity assessment,
        List<QuestionEntity> questionList,
        Dictionary<Guid, string> submittedAnswer
    )> ValidateAndPrepareAsync(
        Guid assessmentId,
        Guid userId,
        AssessmentSubmitRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Validating assessment {AssessmentId} submission for user {UserId}.",
            assessmentId,
            userId
        );

        // Get assessment with questions
        AssessmentEntity? assessment = await _assessmentService.GetAssessmentWithQuestionsAsync(
            assessmentId,
            cancellationToken
        );

        if (assessment is null)
        {
            _logger.LogError("Assessment {AssessmentId} not found for submission.", assessmentId);
            throw new NotFoundException(
                "Assessment not found",
                $"Assessment with ID {assessmentId} does not exist."
            );
        }

        _logger.LogDebug(
            "Found assessment {AssessmentId} with {QuestionCount} questions.",
            assessment.Id,
            assessment.Questions.Count
        );

        // Validate enrollment
        bool isEnrolled = await _assessmentService.IsUserEnrolledAsync(
            userId,
            assessment.CourseId,
            cancellationToken
        );

        if (!isEnrolled)
        {
            _logger.LogError(
                "User {UserId} attempted to submit assessment {AssessmentId} but is not enrolled in course {CourseId}.",
                userId,
                assessment.Id,
                assessment.CourseId
            );
            throw new BadRequestException(
                "Not enrolled",
                "You must be enrolled in the related course to submit an assessment."
            );
        }

        // Validate attempt limit
        IEnumerable<AssessmentHistoryEntity> existingAttempt =
            await _assessmentService.GetUserAssessmentAttemptAsync(
                userId,
                assessment.Id,
                cancellationToken
            );

        int attemptCount = existingAttempt.Count();

        if (attemptCount >= CommonConstants.MaxAssessmentAttempt)
        {
            _logger.LogError(
                "User {UserId} has reached maximum attempts for assessment {AssessmentId}. Attempts: {AttemptCount}.",
                userId,
                assessment.Id,
                attemptCount
            );
            throw new BadRequestException(
                "Maximum attempts reached",
                $"You have reached the maximum of {CommonConstants.MaxAssessmentAttempt} attempts for this assessment."
            );
        }

        // Validate questions
        List<QuestionEntity> questionList = assessment.Questions.ToList();

        if (questionList.Count == 0)
        {
            _logger.LogError(
                "Assessment {AssessmentId} has no questions configured.",
                assessment.Id
            );
            throw new BadRequestException(
                "No questions",
                "The assessment has no questions configured."
            );
        }

        // Build lookup and map submitted answers
        Dictionary<Guid, QuestionEntity> questionLookup = questionList.ToDictionary(q => q.Id);
        Dictionary<Guid, string> submittedAnswer = dto.Answer.ToDictionary(
            a => a.QuestionId,
            a => a.SelectedAnswer
        );

        // Check for invalid question IDs
        List<string> invalidQuestionId = submittedAnswer
            .Keys.Where(id => !questionLookup.ContainsKey(id))
            .Select(id => id.ToString())
            .ToList();

        if (invalidQuestionId.Count != 0)
        {
            _logger.LogError(
                "Assessment {AssessmentId} submission contains invalid question IDs: {InvalidQuestionId}.",
                assessment.Id,
                string.Join(", ", invalidQuestionId)
            );
            throw new BadRequestException(
                "Invalid questions",
                $"The following question IDs are not part of this assessment: {string.Join(", ", invalidQuestionId)}"
            );
        }

        return (assessment, questionList, submittedAnswer);
    }

    private sealed record ComputedSubmissionData(
        DateTime UtcNow,
        int DurationTakenMinutes,
        AssessmentHistoryEntity History,
        List<AssessmentAnswerEntity> AnswerEntityList,
        int CorrectCount,
        int TotalWeight,
        int ObtainedWeight,
        decimal Score,
        decimal WeightedScore,
        decimal CourseMasteryScore,
        int TotalQuestionCount,
        bool Passed,
        RefTermEntity Tier,
        Domain.Entity.User? User
    );

    /// <summary>
    /// Performs all read-only operations: validates start time, evaluates answers,
    /// calculates scores, fetches tier and user data. Returns computed data
    /// that the write phase uses inside the transaction.
    /// </summary>
    private async Task<ComputedSubmissionData> ComputeSubmissionDataAsync(
        AssessmentEntity assessment,
        List<QuestionEntity> questionList,
        Dictionary<Guid, string> submittedAnswer,
        Guid userId,
        DateTime startedOn,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Computing assessment {AssessmentId} submission data for user {UserId}. Questions: {QuestionCount}.",
            assessment.Id,
            userId,
            questionList.Count
        );

        DateTime utcNow = DateTime.UtcNow;

        if (startedOn > utcNow)
        {
            _logger.LogError(
                "Assessment {AssessmentId} submission has future started time: {StartedOn}.",
                assessment.Id,
                startedOn
            );
            throw new BadRequestException(
                "Invalid started time",
                "The assessment start time cannot be in the future."
            );
        }

        int durationTakenMinutes = (int)Math.Ceiling((utcNow - startedOn).TotalMinutes);

        Guid historyId = Guid.NewGuid();
        AssessmentHistoryEntity history = new AssessmentHistoryEntity
        {
            Id = historyId,
            UserId = userId,
            AssessmentId = assessment.Id,
            StartedOn = startedOn,
            CompletedOn = utcNow,
            Score = 0,
            WeightedScore = 0,
        };

        int correctCount = 0;
        int totalWeight = questionList.Sum(q => q.Weight);
        int obtainedWeight = 0;
        List<AssessmentAnswerEntity> answerEntityList = new List<AssessmentAnswerEntity>(
            questionList.Count
        );

        foreach (QuestionEntity question in questionList)
        {
            submittedAnswer.TryGetValue(question.Id, out string? selectedAnswer);
            string selected = selectedAnswer ?? string.Empty;
            bool isCorrect = string.Equals(
                selected.Trim(),
                question.Answer.Trim(),
                StringComparison.OrdinalIgnoreCase
            );

            if (isCorrect)
            {
                correctCount++;
                obtainedWeight += question.Weight;
            }

            answerEntityList.Add(
                new AssessmentAnswerEntity
                {
                    AssessmentHistoryId = historyId,
                    QuestionId = question.Id,
                    SelectedAnswer = selected,
                    IsCorrect = isCorrect,
                    ObtainedScore = isCorrect ? question.Weight : 0,
                }
            );
        }

        decimal score = obtainedWeight;
        decimal weightedScore =
            totalWeight > 0 ? Math.Round((decimal)obtainedWeight / totalWeight * 100, 2) : 0;

        IEnumerable<AssessmentHistoryEntity> courseHistoryList =
            await _assessmentService.GetUserCourseAssessmentHistoriesAsync(
                userId,
                assessment.CourseId,
                cancellationToken
            );

        decimal courseMasteryScore = 0;
        var completedHistories = courseHistoryList.Where(h => h.CompletedOn.HasValue).ToList();
        if (completedHistories.Any())
        {
            var bestScores = completedHistories
                .GroupBy(h => h.AssessmentId)
                .Select(g => g.Max(h => h.WeightedScore))
                .ToList();
            courseMasteryScore = Math.Round(bestScores.Average(), 2);
        }

        bool passed = score >= assessment.PassingMark;

        RefTermEntity? tier = await _assessmentService.GetTierByScoreAsync(
            weightedScore,
            cancellationToken
        );

        if (tier is null)
        {
            _logger.LogError(
                "No tier found for weighted score {WeightedScore} on assessment {AssessmentId}.",
                weightedScore,
                assessment.Id
            );
            throw new NotFoundException(
                "Tier not found",
                $"No tier found for weighted score {weightedScore}. Please ensure tier reference data is seeded."
            );
        }

        Domain.Entity.User? user = await _assessmentService.GetUserByIdAsync(
            userId,
            cancellationToken
        );

        _logger.LogDebug(
            "Tier awarded: {TierName} (Id: {TierId}) for HistoryId: {HistoryId}.",
            tier.Name,
            tier.Id,
            historyId
        );

        return new ComputedSubmissionData(
            UtcNow: utcNow,
            DurationTakenMinutes: durationTakenMinutes,
            History: history,
            AnswerEntityList: answerEntityList,
            CorrectCount: correctCount,
            TotalWeight: totalWeight,
            ObtainedWeight: obtainedWeight,
            Score: score,
            WeightedScore: weightedScore,
            CourseMasteryScore: courseMasteryScore,
            TotalQuestionCount: questionList.Count,
            Passed: passed,
            Tier: tier,
            User: user
        );
    }

    /// <summary>
    /// Computes weak topic data by grouping questions by meta topic and calculating performance metrics.
    /// Returns an empty collection if data is unavailable.
    /// </summary>
    private async Task<List<WeakTopicDto>> ComputeWeakTopicsAsync(
        Guid courseId,
        List<QuestionEntity> questionList,
        List<AssessmentAnswerEntity> answerEntityList,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<CourseMetaTopicEntity> courseMetaTopic =
                await _assessmentService.GetMetaTopicByCourseIdAsync(courseId, cancellationToken);

            if (courseMetaTopic.Count == 0)
            {
                return new List<WeakTopicDto>();
            }

            Dictionary<Guid, string> topicNameMap = courseMetaTopic.ToDictionary(
                mt => mt.Id,
                mt => mt.Name
            );

            List<WeakTopicDto> topicGroup = questionList
                .Where(q => q.MetaTopicId != Guid.Empty)
                .GroupBy(q => q.MetaTopicId)
                .Select(g =>
                {
                    List<AssessmentAnswerEntity> topicAnswer = answerEntityList
                        .Where(a => g.Any(q => q.Id == a.QuestionId))
                        .ToList();

                    int totalWeight = g.Sum(q => q.Weight);
                    int obtainedWeight = topicAnswer.Sum(a => (int)a.ObtainedScore);
                    double averageScore =
                        totalWeight > 0
                            ? Math.Round((double)obtainedWeight / totalWeight * 100, 2)
                            : 0;
                    int failedAttempt = topicAnswer.Count(a => !a.IsCorrect);

                    return new WeakTopicDto
                    {
                        MetaTopicId = g.Key,
                        TopicName = topicNameMap.GetValueOrDefault(g.Key),
                        AverageScore = averageScore,
                        FailedAttempts = failedAttempt,
                    };
                })
                .ToList();

            return topicGroup;
        }
        catch
        {
            _logger.LogError("Failed to compute weak topics. Returning empty collection.");
            return new List<WeakTopicDto>();
        }
    }
}
