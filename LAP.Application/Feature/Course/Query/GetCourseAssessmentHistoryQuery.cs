using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve a paginated list of assessment history for a specific course and user.
/// </summary>
/// <param name="CourseId">The unique identifier of the course.</param>
/// <param name="Page">The page number.</param>
/// <param name="PageSize">The size of the page.</param>
public record GetCourseAssessmentHistoryQuery(Guid CourseId, int Page, int PageSize)
    : IRequest<PaginatedAssessmentHistoryDto>;

/// <summary>
/// Handler for <see cref="GetCourseAssessmentHistoryQuery"/>.
/// </summary>
public class GetCourseAssessmentHistoryHandler
    : IRequestHandler<GetCourseAssessmentHistoryQuery, PaginatedAssessmentHistoryDto>
{
    private readonly ICourseService _courseService;
    private readonly IRequestContext _requestContext;
    private readonly IMapper _mapper;
    private readonly ICustomLogger<GetCourseAssessmentHistoryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseAssessmentHistoryHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseAssessmentHistoryHandler(
        ICourseService courseService,
        IRequestContext requestContext,
        IMapper mapper,
        ICustomLogger<GetCourseAssessmentHistoryHandler> logger
    )
    {
        _courseService = courseService;
        _requestContext = requestContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PaginatedAssessmentHistoryDto> Handle(
        GetCourseAssessmentHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Started fetching assessment history for course {CourseId}.",
            request.CourseId
        );

        Guid userId = _requestContext.UserId ?? Guid.Empty;

        (IEnumerable<AssessmentHistory> item, int totalCount) =
            await _courseService.GetAssessmentHistoryAsync(
                request.CourseId,
                userId,
                request.Page,
                request.PageSize,
                cancellationToken
            );

        _logger.LogInfo(
            "Completed fetching assessment history for course {CourseId}. Total: {TotalCount}",
            request.CourseId,
            totalCount
        );

        return new PaginatedAssessmentHistoryDto
        {
            Data = _mapper.Map<ICollection<AssessmentHistoryDto>>(item),
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
