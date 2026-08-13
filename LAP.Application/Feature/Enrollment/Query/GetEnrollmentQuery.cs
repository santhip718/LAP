using AutoMapper;
using LAP.Application.Constant;
using LAP.Application.DTO.Enrollment;
using LAP.Application.DTO.Paginated;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using MediatR;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.Application.Feature.Enrollment.Query;

/// <summary>
/// Query for retrieving a paginated list of enrollments with optional filters.
/// </summary>
/// <param name="CourseName">Optional course name filter.</param>
/// <param name="CategoryId">Optional category identifier filter.</param>
/// <param name="Page">The page number to retrieve.</param>
/// <param name="PageSize">The number of items per page.</param>
public record GetEnrollmentQuery(string? CourseName, Guid? CategoryId, int Page, int PageSize)
    : IRequest<PaginatedEnrollmentsDto>;

/// <summary>
/// Handles the <see cref="GetEnrollmentQuery"/> by fetching and mapping paginated enrollment data.
/// </summary>
public class GetEnrollmentHandler : IRequestHandler<GetEnrollmentQuery, PaginatedEnrollmentsDto>
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICustomLogger<GetEnrollmentHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEnrollmentHandler"/> class.
    /// </summary>
    /// <param name="enrollmentService">Service used to retrieve enrollment data.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping enrollment entities to DTOs.</param>
    /// <param name="requestContext">Request context with authenticated user info.</param>
    public GetEnrollmentHandler(
        IEnrollmentService enrollmentService,
        ICustomLogger<GetEnrollmentHandler> logger,
        IMapper mapper,
        IRequestContext requestContext
    )
    {
        _enrollmentService = enrollmentService;
        _logger = logger;
        _mapper = mapper;
        _requestContext = requestContext;
    }

    /// <summary>
    /// Handles the enrollment retrieval request and returns a paginated result with enrollment details.
    /// </summary>
    /// <param name="request">The <see cref="GetEnrollmentQuery"/> containing filter and pagination parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="PaginatedEnrollmentsDto"/> containing the filtered enrollment details for the requested page.
    /// </returns>
    public async Task<PaginatedEnrollmentsDto> Handle(
        GetEnrollmentQuery request,
        CancellationToken cancellationToken
    )
    {
        Guid? effectiveUserId = string.Equals(
            _requestContext.Role,
            RoleConstants.ADMIN_ROLE_NAME,
            StringComparison.OrdinalIgnoreCase
        )
            ? null
            : _requestContext.UserId;

        _logger.LogInfo(
            "Fetching enrollments for course name {CourseName}, category {CategoryId}, user {UserId}, page {Page}, and page size {PageSize}.",
            request.CourseName,
            request.CategoryId,
            effectiveUserId,
            request.Page,
            request.PageSize
        );

        List<EnrollmentEntity> all = await _enrollmentService.GetEnrollmentAsync(
            request.CourseName,
            request.CategoryId,
            effectiveUserId,
            cancellationToken
        );

        int total = all.Count;

        List<EnrollmentEntity> paged = all.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        List<EnrollmentDetailDto> mapped = _mapper.Map<List<EnrollmentDetailDto>>(paged);

        _logger.LogInfo(
            "Returning {Count} enrollment(s) for page {Page} out of {Total} total.",
            mapped.Count,
            request.Page,
            total
        );

        return new PaginatedEnrollmentsDto
        {
            Data = mapped,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
