using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseReview.Command;

/// <summary>
/// Command to delete a course review.
/// </summary>
/// <param name="Id">The unique identifier of the review to delete.</param>
public record DeleteReviewCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="DeleteReviewCommand"/> request.
/// </summary>
public class DeleteReviewValidator : AbstractValidator<DeleteReviewCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteReviewValidator"/> class.
    /// </summary>
    public DeleteReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Review identifier is required");
    }
}

/// <summary>
/// Handles the deletion (soft delete) of a course review.
/// </summary>
public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, SuccessResponse>
{
    private readonly IReviewService _reviewService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<DeleteReviewHandler> _logger;
    private readonly IRequestContext _requestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteReviewHandler"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="requestContext">The current request context.</param>
    public DeleteReviewHandler(
        IReviewService reviewService,
        ITransactionService transactionService,
        ICustomLogger<DeleteReviewHandler> logger,
        IRequestContext requestContext
    )
    {
        _reviewService = reviewService;
        _transactionService = transactionService;
        _logger = logger;
        _requestContext = requestContext;
    }

    /// <summary>
    /// Processes the deletion of a review, ensuring ownership and validity.
    /// </summary>
    /// <param name="request">The delete review command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="SuccessResponse"/> indicating the result of the deletion.</returns>
    /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
    /// <exception cref="NotFoundException">Thrown if the review does not exist.</exception>
    /// <exception cref="ForbiddenException">Thrown if the user is not the owner of the review.</exception>
    public async Task<SuccessResponse> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Processing review deletion for review {ReviewId} by user {UserId}.",
            request.Id,
            userId
        );

        // Read phase: validate and prepare outside transaction
        Review? review = await _reviewService.GetReviewByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            _logger.LogError("Review deletion failed. Review {ReviewId} not found.", request.Id);
            throw new NotFoundException(
                "Review not found",
                $"Review with ID {request.Id} does not exist."
            );
        }

        if (review.UserId != userId)
        {
            _logger.LogError(
                "Review deletion failed. User {UserId} is not the owner of review {ReviewId}.",
                userId,
                request.Id
            );
            throw new ForbiddenException("Access denied", "You can only delete your own reviews.");
        }

        Guid courseId = review.CourseId;
        int deletedRating = review.Rating;

        // Pre-fetch course and reviews for rating calculation
        LAP.Domain.Entity.Course? course = await _reviewService.GetCourseByIdAsync(
            courseId,
            cancellationToken
        );

        decimal newOverallRating = 0;
        if (course != null)
        {
            IEnumerable<Review> allReviews = await _reviewService.GetReviewByCourseIdAsync(
                courseId,
                cancellationToken
            );

            var reviewList = allReviews.ToList();
            int remainingCount = reviewList.Count - 1;

            if (remainingCount > 0)
            {
                decimal totalSum = reviewList.Sum(r => r.Rating);
                decimal adjustedSum = totalSum - deletedRating;
                newOverallRating = Math.Round(adjustedSum / remainingCount, 2);
            }
        }

        // Write phase: only data modification inside transaction
        int affectedRow = 0;
        await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                affectedRow = await _reviewService.DeleteReviewAsync(request.Id, cancellationToken);

                if (affectedRow > 0 && course != null)
                {
                    IEnumerable<Review> reviewList = await _reviewService.GetReviewByCourseIdAsync(
                        courseId,
                        cancellationToken
                    );
                    course.OverallRating = reviewList.Any()
                        ? (decimal)reviewList.Average(r => r.Rating)
                        : 0;

                    await _reviewService.UpdateCourseAsync(course, cancellationToken);
                    await _transactionService.SaveChangesAsync(cancellationToken);
                }
            },
            cancellationToken
        );

        if (affectedRow == 0)
        {
            _logger.LogError(
                "Review deletion failed. Review {ReviewId} was not found during deletion.",
                request.Id
            );
            throw new NotFoundException(
                "Review not found",
                $"Review with ID {request.Id} was not found and could not be deleted."
            );
        }

        _logger.LogInfo(
            "Review {ReviewId} deleted successfully by user {UserId}.",
            review.Id,
            userId
        );

        return new SuccessResponse { Id = review.Id, Message = "Review deleted successfully" };
    }
}
