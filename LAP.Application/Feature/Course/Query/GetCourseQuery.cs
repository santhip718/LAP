using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.Paginated;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve a paginated and filtered list of courses.
/// </summary>
/// <param name="Page">The page number (default 1).</param>
/// <param name="PageSize">The number of records per page (default 20).</param>
/// <param name="CategoryId">Optional category filter.</param>
/// <param name="DifficultyLevelId">Optional difficulty level filter.</param>
/// <param name="Status">Optional status filter (true for published, false for drafted).</param>
/// <param name="Search">Optional search string for title.</param>
public record GetCourseQuery(
    int Page,
    int PageSize,
    Guid? CategoryId,
    Guid? DifficultyLevelId,
    bool? Status,
    string? Search
) : IRequest<PaginatedCoursesDto>;

/// <summary>
/// Validates the <see cref="GetCourseQuery"/> parameters.
/// </summary>
public class GetCourseQueryValidator : AbstractValidator<GetCourseQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseQueryValidator"/> class.
    /// </summary>
    public GetCourseQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}

/// <summary>
/// Handles the retrieval of paginated and filtered courses.
/// </summary>
public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, PaginatedCoursesDto>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetCourseQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseQueryHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="fileStorageService">The file storage service for Base64 conversion.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseQueryHandler(
        ICourseService courseService,
        IMapper mapper,
        IFileStorageService fileStorageService,
        ICustomLogger<GetCourseQueryHandler> logger
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Processes the request to get paged courses.
    /// </summary>
    /// <param name="request">The get course query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="PaginatedCoursesDto"/> containing the paginated list of courses.</returns>
    public async Task<PaginatedCoursesDto> Handle(
        GetCourseQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Started fetching paged courses for page {Page} with page size {PageSize}.",
            request.Page,
            request.PageSize
        );

        (IEnumerable<Domain.Entity.Course> item, int totalCount) =
            await _courseService.GetPagedCoursesAsync(
                request.Page,
                request.PageSize,
                request.CategoryId,
                request.DifficultyLevelId,
                request.Status,
                request.Search,
                cancellationToken
            );

        ICollection<CourseSummaryDto> dto = _mapper.Map<ICollection<CourseSummaryDto>>(item);
        foreach (CourseSummaryDto courseDto in dto)
        {
            courseDto.ThumbnailImg = await _fileStorageService.GetBase64Async(courseDto.ThumbnailImg);
        }

        PaginatedCoursesDto result = new PaginatedCoursesDto
        {
            Data = dto,
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };

        _logger.LogInfo(
            "Completed fetching paged courses for page {Page} with page size {PageSize}.",
            request.Page,
            request.PageSize
        );

        return result;
    }
}
