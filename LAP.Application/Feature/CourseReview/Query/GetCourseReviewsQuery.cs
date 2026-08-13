using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.Review;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseReview.Query;

/// <summary>
/// Query to retrieve paginated reviews for a specific course.
/// </summary>
/// <param name="CourseId">The unique identifier of the course.</param>
/// <param name="Page">The page number to retrieve.</param>
/// <param name="PageSize">The number of items per page.</param>
public record GetCourseReviewsQuery(Guid CourseId, int Page = 1, int PageSize = 10)
    : IRequest<PaginatedReviewsDto>;

/// <summary>
/// Validates the <see cref="GetCourseReviewsQuery"/> request.
/// </summary>
public class GetCourseReviewsValidator : AbstractValidator<GetCourseReviewsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseReviewsValidator"/> class.
    /// </summary>
    public GetCourseReviewsValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course identifier is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");
    }
}

/// <summary>
/// Handles the retrieval of paginated reviews for a course.
/// </summary>
public class GetCourseReviewsHandler : IRequestHandler<GetCourseReviewsQuery, PaginatedReviewsDto>
{
    private readonly IReviewService _reviewService;
    private readonly ICustomLogger<GetCourseReviewsHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseReviewsHandler"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetCourseReviewsHandler(
        IReviewService reviewService,
        ICustomLogger<GetCourseReviewsHandler> logger,
        IMapper mapper
    )
    {
        _reviewService = reviewService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves paginated reviews for the specified course.
    /// </summary>
    /// <param name="request">The get course reviews query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of review details.</returns>
    /// <exception cref="NotFoundException">Thrown if the course does not exist.</exception>
    public async Task<PaginatedReviewsDto> Handle(
        GetCourseReviewsQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Started fetching reviews for course {CourseId} page {Page} with page size {PageSize}.",
            request.CourseId,
            request.Page,
            request.PageSize
        );

        LAP.Domain.Entity.Course? course = await _reviewService.GetCourseByIdAsync(
            request.CourseId,
            cancellationToken
        );
        if (course == null)
        {
            _logger.LogError(
                "Fetch reviews failed. Course {CourseId} not found.",
                request.CourseId
            );
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.CourseId} does not exist."
            );
        }

        (IEnumerable<Review> item, int totalCount) =
            await _reviewService.GetPagedReviewsByCourseIdAsync(
                request.CourseId,
                request.Page,
                request.PageSize,
                cancellationToken
            );

        ICollection<ReviewDto> dto = _mapper.Map<ICollection<ReviewDto>>(item);

        PaginatedReviewsDto result = new PaginatedReviewsDto
        {
            Data = dto,
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };

        _logger.LogInfo(
            "Completed fetching reviews for course {CourseId} page {Page} with page size {PageSize}.",
            request.CourseId,
            request.Page,
            request.PageSize
        );

        return result;
    }
}
