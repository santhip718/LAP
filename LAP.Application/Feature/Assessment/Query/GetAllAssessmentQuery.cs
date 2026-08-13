using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to retrieve all assessments with pagination.
/// </summary>
/// <param name="PageNumber">The page number (1-based).</param>
/// <param name="PageSize">The number of records per page.</param>
public record GetAllAssessmentQuery(int PageNumber, int PageSize)
    : IRequest<PaginatedAssessmentsDto>;

/// <summary>
/// Validator for <see cref="GetAllAssessmentQuery"/>.
/// </summary>
public class GetAllAssessmentValidator : AbstractValidator<GetAllAssessmentQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllAssessmentValidator"/> class.
    /// </summary>
    public GetAllAssessmentValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

/// <summary>
/// Handler for <see cref="GetAllAssessmentQuery"/>.
/// </summary>
public class GetAllAssessmentHandler
    : IRequestHandler<GetAllAssessmentQuery, PaginatedAssessmentsDto>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<GetAllAssessmentHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllAssessmentHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAllAssessmentHandler(
        IAssessmentService assessmentService,
        ICustomLogger<GetAllAssessmentHandler> logger,
        IMapper mapper
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve all assessments with pagination.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of assessment overviews.</returns>
    public async Task<PaginatedAssessmentsDto> Handle(
        GetAllAssessmentQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Started retrieving assessments (page {PageNumber}, size {PageSize})",
            request.PageNumber,
            request.PageSize
        );

        (List<LAP.Domain.Entity.Assessment> assessments, int totalCount) =
            await _assessmentService.GetAllAssessmentPaginatedAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

        List<AssessmentOverviewDto> data = _mapper.Map<List<AssessmentOverviewDto>>(assessments);

        PaginatedAssessmentsDto response = new PaginatedAssessmentsDto
        {
            Data = data,
            Total = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
        };

        _logger.LogInfo("Completed retrieving assessments");

        return response;
    }
}
