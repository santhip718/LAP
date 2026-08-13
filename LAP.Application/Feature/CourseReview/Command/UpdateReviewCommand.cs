using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Review;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseReview.Command;

/// <summary>
/// Command to update an existing course review.
/// </summary>
/// <param name="Id">The unique identifier of the review.</param>
/// <param name="Dto">The updated review details.</param>
public record UpdateReviewCommand(Guid Id, UpdateReviewRequestDto Dto) : IRequest<ReviewDto>;

/// <summary>
/// Validates the <see cref="UpdateReviewCommand"/> request data.
/// </summary>
public class UpdateReviewValidator : AbstractValidator<UpdateReviewCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReviewValidator"/> class.
    /// </summary>
    public UpdateReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Review identifier is required");

        RuleFor(x => x.Dto.ReviewText)
            .MaximumLength(1000)
            .WithMessage("Review text cannot exceed 1000 characters");
    }
}

/// <summary>
/// Handles the update of an existing course review.
/// </summary>
public class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IReviewService _reviewService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<UpdateReviewHandler> _logger;
    private readonly IRequestContext _requestContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReviewHandler"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public UpdateReviewHandler(
        IReviewService reviewService,
        ITransactionService transactionService,
        ICustomLogger<UpdateReviewHandler> logger,
        IRequestContext requestContext,
        IMapper mapper
    )
    {
        _reviewService = reviewService;
        _transactionService = transactionService;
        _logger = logger;
        _requestContext = requestContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Processes the update of an existing review, ensuring ownership and validity.
    /// </summary>
    /// <param name="request">The update review command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated review details.</returns>
    /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
    /// <exception cref="NotFoundException">Thrown if the review does not exist.</exception>
    /// <exception cref="ForbiddenException">Thrown if the user is not the owner of the review.</exception>
    public async Task<ReviewDto> Handle(
        UpdateReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Processing review update for review {ReviewId} by user {UserId}.",
            request.Id,
            userId
        );

        // Read phase: validate and prepare outside transaction
        Review? review = await _reviewService.GetReviewByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            _logger.LogError("Review update failed. Review {ReviewId} not found.", request.Id);
            throw new NotFoundException(
                "Review not found",
                $"Review with ID {request.Id} does not exist."
            );
        }

        if (review.UserId != userId)
        {
            _logger.LogError(
                "Review update failed. User {UserId} is not the owner of review {ReviewId}.",
                userId,
                request.Id
            );
            throw new ForbiddenException("Access denied", "You can only update your own reviews.");
        }

        if (
            request.Dto.Rating.HasValue
            && (request.Dto.Rating.Value < 1 || request.Dto.Rating.Value > 5)
        )
        {
            _logger.LogError(
                "Review update failed. Rating {Rating} is out of range.",
                request.Dto.Rating.Value
            );
            throw new BadRequestException("Invalid rating", "Rating must be between 1 and 5.");
        }

        int oldRating = review.Rating;
        _mapper.Map(request.Dto, review);

        // Pre-fetch course and reviews for rating calculation
        LAP.Domain.Entity.Course? course = await _reviewService.GetCourseByIdAsync(
            review.CourseId,
            cancellationToken
        );

        decimal newOverallRating = 0;
        if (course != null)
        {
            IEnumerable<Review> allReviews = await _reviewService.GetReviewByCourseIdAsync(
                review.CourseId,
                cancellationToken
            );

            var reviewList = allReviews.ToList();
            decimal totalSum = reviewList.Sum(r => r.Rating);
            int newRating = request.Dto.Rating ?? oldRating;
            int count = reviewList.Count;
            decimal adjustedSum = totalSum - oldRating + newRating;
            newOverallRating = count > 0 ? Math.Round(adjustedSum / count, 2) : 0;
        }

        // Write phase: only data modification inside transaction
        ReviewDto result = await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                await _reviewService.UpdateReviewAsync(review, cancellationToken);
                await _transactionService.SaveChangesAsync(cancellationToken);

                if (course != null)
                {
                    IEnumerable<Review> reviewList = await _reviewService.GetReviewByCourseIdAsync(
                        review.CourseId,
                        cancellationToken
                    );
                    course.OverallRating = reviewList.Any()
                        ? (decimal)reviewList.Average(r => r.Rating)
                        : 0;

                    await _reviewService.UpdateCourseAsync(course, cancellationToken);
                    await _transactionService.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInfo(
                    "Review {ReviewId} updated successfully by user {UserId}.",
                    review.Id,
                    userId
                );

                return _mapper.Map<ReviewDto>(review);
            },
            cancellationToken
        );

        _logger.LogInfo(
            "Finished processing review update for review {ReviewId} by user {UserId}.",
            request.Id,
            userId
        );

        return result;
    }
}
