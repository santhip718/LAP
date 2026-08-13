using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Review;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseReview.Query;

/// <summary>
/// Query to retrieve a specific user's review for a specific course.
/// </summary>
/// <param name="CourseId">The unique identifier of the course.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record GetUserCourseReviewQuery(Guid CourseId, Guid UserId) : IRequest<ReviewDto>;

/// <summary>
/// Validates the <see cref="GetUserCourseReviewQuery"/> request.
/// </summary>
public class GetUserCourseReviewValidator : AbstractValidator<GetUserCourseReviewQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserCourseReviewValidator"/> class.
    /// </summary>
    public GetUserCourseReviewValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course identifier is required");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of a specific user's review for a course.
/// </summary>
public class GetUserCourseReviewHandler : IRequestHandler<GetUserCourseReviewQuery, ReviewDto>
{
    private readonly IReviewService _reviewService;
    private readonly ICustomLogger<GetUserCourseReviewHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserCourseReviewHandler"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetUserCourseReviewHandler(
        IReviewService reviewService,
        ICustomLogger<GetUserCourseReviewHandler> logger,
        IMapper mapper
    )
    {
        _reviewService = reviewService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves the review for the specified course and user.
    /// </summary>
    /// <param name="request">The get user course review query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The review details.</returns>
    /// <exception cref="NotFoundException">Thrown if the review or course does not exist.</exception>
    public async Task<ReviewDto> Handle(
        GetUserCourseReviewQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Fetching review for course {CourseId} by user {UserId}.",
            request.CourseId,
            request.UserId
        );

        // Check if course exists
        LAP.Domain.Entity.Course? course = await _reviewService.GetCourseByIdAsync(
            request.CourseId,
            cancellationToken
        );
        if (course == null)
        {
            _logger.LogError("Fetch review failed. Course {CourseId} not found.", request.CourseId);
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.CourseId} does not exist."
            );
        }
        Review? review = await _reviewService.GetUserReviewForCourseAsync(
            request.CourseId,
            request.UserId,
            cancellationToken
        );

        if (review == null)
        {
            _logger.LogError(
                "Review not found for course {CourseId} and user {UserId}.",
                request.CourseId,
                request.UserId
            );
            throw new NotFoundException(
                "Review not found",
                "You have not reviewed this course yet."
            );
        }

        ReviewDto result = _mapper.Map<ReviewDto>(review);

        _logger.LogInfo(
            "Finished fetching review for course {CourseId} by user {UserId}.",
            request.CourseId,
            request.UserId
        );

        return result;
    }
}
