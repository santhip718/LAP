using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Review;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseReview.Command;

/// <summary>
/// Command to create a new review for a course.
/// </summary>
/// <param name="CourseId">The identifier of the course being reviewed.</param>
/// <param name="Dto">The review details.</param>
public record CreateReviewCommand(Guid CourseId, CreateReviewRequestDto Dto) : IRequest<ReviewDto>;

/// <summary>
/// Validates the <see cref="CreateReviewCommand"/> request data.
/// </summary>
public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateReviewValidator"/> class.
    /// </summary>
    public CreateReviewValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course identifier is required");

        RuleFor(x => x.Dto).NotNull().WithMessage("Review details are required");

        When(
            x => x.Dto is not null,
            () =>
            {
                RuleFor(x => x.Dto.Rating)
                    .InclusiveBetween(1, 5)
                    .WithMessage("Rating must be between 1 and 5");

                RuleFor(x => x.Dto.ReviewText)
                    .MaximumLength(1000)
                    .WithMessage("Review text cannot exceed 1000 characters");

                RuleFor(x => x.Dto.ReviewText)
                    .Must(text => string.IsNullOrWhiteSpace(text) || text.Trim().Length > 0)
                    .WithMessage("Review text cannot contain only whitespace");
            }
        );
    }
}

/// <summary>
/// Handles the creation of a new course review.
/// </summary>
public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewService _reviewService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<CreateReviewHandler> _logger;
    private readonly IRequestContext _requestContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateReviewHandler"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public CreateReviewHandler(
        IReviewService reviewService,
        ITransactionService transactionService,
        ICustomLogger<CreateReviewHandler> logger,
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
    /// Creates a review for the specified course.
    /// </summary>
    /// <param name="request">The review creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created review.</returns>
    public async Task<ReviewDto> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Started review creation for course {CourseId} by user {UserId}.",
            request.CourseId,
            userId
        );

        // Read phase: validate and prepare outside transaction
        LAP.Domain.Entity.Course? course = await _reviewService.GetCourseByIdAsync(
            request.CourseId,
            cancellationToken
        );

        if (course is null)
        {
            _logger.LogError("Course {CourseId} not found for review creation.", request.CourseId);
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.CourseId} does not exist."
            );
        }

        LAP.Domain.Entity.Enrollment? enrollment = await _reviewService.GetUserEnrollmentAsync(
            userId,
            request.CourseId,
            cancellationToken
        );

        if (enrollment is null)
        {
            _logger.LogError(
                "User {UserId} attempted to review course {CourseId} but is not enrolled.",
                userId,
                request.CourseId
            );
            throw new BadRequestException(
                "Not enrolled",
                "You must be enrolled in the course to submit a review."
            );
        }

        if (!enrollment.EnrollmentStatus)
        {
            _logger.LogError(
                "User {UserId} attempted to review course {CourseId} but enrollment is not approved.",
                userId,
                request.CourseId
            );
            throw new BadRequestException(
                "Enrollment not approved",
                "Your enrollment has not been approved yet. Please wait for admin approval."
            );
        }

        bool hasReviewed = await _reviewService.HasUserReviewedAsync(
            userId,
            request.CourseId,
            cancellationToken
        );

        if (hasReviewed)
        {
            _logger.LogError(
                "User {UserId} has already reviewed course {CourseId}.",
                userId,
                request.CourseId
            );
            throw new ConflictException(
                "Review already exists",
                "You have already submitted a review for this course."
            );
        }

        Review review = _mapper.Map<Review>(request.Dto);
        review.UserId = userId;
        review.CourseId = request.CourseId;
        review.ReviewText = review.ReviewText?.Trim();

        // Pre-fetch existing reviews for rating calculation
        IEnumerable<Review> existingReviews = await _reviewService.GetReviewByCourseIdAsync(
            request.CourseId,
            cancellationToken
        );

        var reviewList = existingReviews.ToList();
        int existingCount = reviewList.Count;
        decimal existingSum = reviewList.Sum(r => r.Rating);
        int newTotalCount = existingCount + 1;
        decimal newSum = existingSum + review.Rating;
        decimal newOverallRating = newTotalCount > 0 ? Math.Round(newSum / newTotalCount, 2) : 0;

        // Write phase: only data modification inside transaction
        ReviewDto reviewDto = await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                await _reviewService.AddReviewAsync(review, cancellationToken);

                await _transactionService.SaveChangesAsync(cancellationToken);

                course.OverallRating = newOverallRating;

                await _reviewService.UpdateCourseAsync(course, cancellationToken);

                await _transactionService.SaveChangesAsync(cancellationToken);

                _logger.LogInfo(
                    "Review {ReviewId} created successfully for course {CourseId} by user {UserId}.",
                    review.Id,
                    request.CourseId,
                    userId
                );

                return _mapper.Map<ReviewDto>(review);
            },
            cancellationToken
        );

        _logger.LogInfo(
            "Completed review creation for course {CourseId} by user {UserId}.",
            request.CourseId,
            userId
        );

        return reviewDto;
    }
}
