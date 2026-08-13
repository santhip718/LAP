using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to retrieve all assessment results for a specific assessment.
/// </summary>
/// <param name="AssessmentId">The identifier of the assessment.</param>
public record GetAssessmentResultQuery(Guid AssessmentId) : IRequest<AssessmentResultResponseDto>;

/// <summary>
/// Validates the <see cref="GetAssessmentResultQuery"/> request data.
/// </summary>
public class GetAssessmentResultValidator : AbstractValidator<GetAssessmentResultQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssessmentResultValidator"/> class.
    /// </summary>
    public GetAssessmentResultValidator()
    {
        RuleFor(x => x.AssessmentId).NotEmpty().WithMessage("Assessment identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of all assessment results for the authenticated user.
/// </summary>
public class GetAssessmentResultHandler
    : IRequestHandler<GetAssessmentResultQuery, AssessmentResultResponseDto>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<GetAssessmentResultHandler> _logger;
    private readonly IRequestContext _requestContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssessmentResultHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAssessmentResultHandler(
        IAssessmentService assessmentService,
        ICustomLogger<GetAssessmentResultHandler> logger,
        IRequestContext requestContext,
        IMapper mapper
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
        _requestContext = requestContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all assessment results for the logged-in user.
    /// </summary>
    /// <param name="request">The get assessment result query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The assessment result details with all attempts.</returns>
    public async Task<AssessmentResultResponseDto> Handle(
        GetAssessmentResultQuery request,
        CancellationToken cancellationToken
    )
    {
        Guid authenticatedUserId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Retrieving assessment results for assessment {AssessmentId} by user {UserId}.",
            request.AssessmentId,
            authenticatedUserId
        );

        Domain.Entity.Assessment? assessment = await _assessmentService.GetAssessmentByIdAsync(
            request.AssessmentId,
            cancellationToken
        );

        if (assessment is null)
        {
            _logger.LogError(
                "Assessment {AssessmentId} not found for result retrieval.",
                request.AssessmentId
            );
            throw new NotFoundException(
                "Assessment not found",
                $"Assessment with ID {request.AssessmentId} does not exist."
            );
        }

        IEnumerable<Domain.Entity.AssessmentHistory> histories =
            await _assessmentService.GetAllAssessmentHistoriesAsync(
                request.AssessmentId,
                authenticatedUserId,
                cancellationToken
            );

        if (!histories.Any())
        {
            _logger.LogError(
                "No completed attempts found for assessment {AssessmentId} by user {UserId}.",
                request.AssessmentId,
                authenticatedUserId
            );
            throw new NotFoundException(
                "Result not found",
                "No completed assessment attempt was found for this assessment."
            );
        }

        List<AssessmentAttemptDto> attempts = histories
            .Select(
                (history, index) =>
                    new AssessmentAttemptDto
                    {
                        AttemptNumber = index + 1,
                        AttemptedOn = history.CompletedOn ?? history.StartedOn,
                        Score = history.Score,
                        WeightedScore = history.WeightedScore,
                        Passed = history.Score >= assessment.PassingMark,
                    }
            )
            .ToList();

        AssessmentResultResponseDto result = new AssessmentResultResponseDto
        {
            AssessmentId = assessment.Id,
            AssessmentTitle = assessment.Title,
            PassingMark = assessment.PassingMark,
            Attempts = attempts,
        };

        _logger.LogInfo(
            "Successfully retrieved {AttemptCount} assessment result(s) for assessment {AssessmentId} by user {UserId}.",
            attempts.Count,
            request.AssessmentId,
            authenticatedUserId
        );

        return result;
    }
}
