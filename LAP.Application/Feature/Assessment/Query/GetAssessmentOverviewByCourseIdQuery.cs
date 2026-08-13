using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to retrieve assessment overviews for a specific course.
/// </summary>
/// <param name="CourseId">The course identifier.</param>
public record GetAssessmentOverviewByCourseIdQuery(Guid CourseId)
    : IRequest<List<AssessmentOverviewDto>>;

/// <summary>
/// Validator for <see cref="GetAssessmentOverviewByCourseIdQuery"/>.
/// </summary>
public class GetAssessmentOverviewByCourseIdValidator
    : AbstractValidator<GetAssessmentOverviewByCourseIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssessmentOverviewByCourseIdValidator"/> class.
    /// </summary>
    public GetAssessmentOverviewByCourseIdValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");
    }
}

/// <summary>
/// Handler for <see cref="GetAssessmentOverviewByCourseIdQuery"/>.
/// </summary>
public class GetAssessmentOverviewByCourseIdHandler
    : IRequestHandler<GetAssessmentOverviewByCourseIdQuery, List<AssessmentOverviewDto>>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<GetAssessmentOverviewByCourseIdHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssessmentOverviewByCourseIdHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAssessmentOverviewByCourseIdHandler(
        IAssessmentService assessmentService,
        ICustomLogger<GetAssessmentOverviewByCourseIdHandler> logger,
        IMapper mapper
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve assessment overviews for a specific course.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of assessment overviews for the course.</returns>
    public async Task<List<AssessmentOverviewDto>> Handle(
        GetAssessmentOverviewByCourseIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Assessment overview retrieval started for course {CourseId}",
            request.CourseId
        );

        List<LAP.Domain.Entity.Assessment> assessment = await _assessmentService.GetByCourseIdAsync(
            request.CourseId,
            cancellationToken
        );

        List<AssessmentOverviewDto> response = _mapper.Map<List<AssessmentOverviewDto>>(assessment);

        _logger.LogInfo(
            "Assessment overview retrieval completed successfully for course {CourseId}",
            request.CourseId
        );

        return response;
    }
}
