using AutoMapper;
using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to retrieve all questions for a specific assessment.
/// </summary>
/// <param name="AssessmentId">The assessment identifier.</param>
public record GetQuestionByAssessmentIdQuery(Guid AssessmentId) : IRequest<List<QuestionDto>>;

/// <summary>
/// Validator for <see cref="GetQuestionByAssessmentIdQuery"/>.
/// </summary>
public class GetQuestionByAssessmentIdValidator : AbstractValidator<GetQuestionByAssessmentIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetQuestionByAssessmentIdValidator"/> class.
    /// </summary>
    public GetQuestionByAssessmentIdValidator()
    {
        RuleFor(x => x.AssessmentId).NotEmpty().WithMessage("Assessment ID is required");
    }
}

/// <summary>
/// Handler for <see cref="GetQuestionByAssessmentIdQuery"/>.
/// </summary>
public class GetQuestionByAssessmentIdHandler
    : IRequestHandler<GetQuestionByAssessmentIdQuery, List<QuestionDto>>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<GetQuestionByAssessmentIdHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly IPermissionCacheService _permissionCacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetQuestionByAssessmentIdHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="requestContext">The request context for the current user.</param>
    /// <param name="permissionCacheService">The permission cache service for feature authorization checks.</param>
    public GetQuestionByAssessmentIdHandler(
        IAssessmentService assessmentService,
        ICustomLogger<GetQuestionByAssessmentIdHandler> logger,
        IMapper mapper,
        IRequestContext requestContext,
        IPermissionCacheService permissionCacheService
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
        _mapper = mapper;
        _requestContext = requestContext;
        _permissionCacheService = permissionCacheService;
    }

    /// <summary>
    /// Handles the request to retrieve questions for a specific assessment.
    /// Returns answers only for users with the MANAGE_ASSESSMENT feature authorization.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of questions for the assessment.</returns>
    public async Task<List<QuestionDto>> Handle(
        GetQuestionByAssessmentIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Question retrieval started for assessment {AssessmentId}",
            request.AssessmentId
        );

        LAP.Domain.Entity.Assessment? assessment = await _assessmentService.GetAssessmentByIdAsync(
            request.AssessmentId,
            cancellationToken
        );
        if (assessment == null)
        {
            _logger.LogError(
                "Assessment not found for assessment ID {AssessmentId}",
                request.AssessmentId
            );
            throw new NotFoundException(
                "Assessment not found",
                $"The assessment with ID {request.AssessmentId} does not exist."
            );
        }

        List<Question> question = await _assessmentService.GetQuestionByAssessmentIdAsync(
            request.AssessmentId,
            cancellationToken
        );

        List<QuestionDto> response = _mapper.Map<List<QuestionDto>>(question);

        bool hasManageAccess = await UserHasManageAssessmentAccessAsync(cancellationToken);
        if (!hasManageAccess)
        {
            foreach (QuestionDto questionDto in response)
            {
                questionDto.Answer = null;
            }
        }

        _logger.LogInfo(
            "Question retrieval completed successfully for assessment {AssessmentId}",
            request.AssessmentId
        );

        return response;
    }

    /// <summary>
    /// Checks whether the current user has the MANAGE_ASSESSMENT feature permission.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the user has the permission; otherwise, <c>false</c>.</returns>
    private async Task<bool> UserHasManageAssessmentAccessAsync(CancellationToken cancellationToken)
    {
        string? role = _requestContext.Role;
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        HashSet<string> permissions = await _permissionCacheService.GetPermissionsAsync(
            role,
            cancellationToken
        );
        return permissions.Contains("MANAGE_ASSESSMENT");
    }
}
