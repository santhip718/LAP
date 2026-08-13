using LAP.API.Authorization;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.Review;
using LAP.Application.Feature.CourseReview.Command;
using LAP.Application.Feature.CourseReview.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Provides endpoints for managing course reviews, including creating, updating, deleting, and retrieving reviews.
/// </summary>
[Route("api/v1/review")]
[Authorize]
public class ReviewController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<ReviewController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for dispatching commands and queries.</param>
    /// <param name="logger">The application logger.</param>
    public ReviewController(IMediator mediator, ICustomLogger<ReviewController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new review for a specified course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="request">The review details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created review details.</returns>
    [HttpPost("course/{course-id}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The created review details.", typeof(ReviewDto))]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid review details or not enrolled.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status409Conflict,
        "Review already exists.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("CREATE_REVIEW")]
    public async Task<IActionResult> CreateReview(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromBody] CreateReviewRequestDto request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received create review request for course {CourseId}.", courseId);
        ReviewDto result = await _mediator.Send(
            new CreateReviewCommand(courseId, request),
            cancellationToken
        );
        _logger.LogDebug("Finished create review request for course {CourseId}.", courseId);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing course review.
    /// </summary>
    /// <param name="id">The unique identifier of the review.</param>
    /// <param name="request">The updated review details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated review details.</returns>
    [HttpPut("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The updated review details.", typeof(ReviewDto))]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid review details.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission or is not the owner.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Review not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("MANAGE_REVIEW")]
    public async Task<IActionResult> UpdateReview(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] UpdateReviewRequestDto request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received update review request for review {Id}.", id);
        ReviewDto result = await _mediator.Send(
            new UpdateReviewCommand(id, request),
            cancellationToken
        );
        _logger.LogDebug("Finished update review request for review {Id}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a course review.
    /// </summary>
    /// <param name="id">The unique identifier of the review.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="SuccessResponse"/> indicating the result of the deletion.</returns>
    [HttpDelete("{id}")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Indicates the result of the deletion.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission or is not the owner.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Review not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("MANAGE_REVIEW")]
    public async Task<IActionResult> DeleteReview(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received delete review request for review {Id}.", id);
        SuccessResponse result = await _mediator.Send(
            new DeleteReviewCommand(id),
            cancellationToken
        );
        _logger.LogDebug("Finished delete review request for review {Id}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of reviews for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="page">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10, max: 100).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of reviews for the course.</returns>
    [HttpGet("course/{course-id}")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A paginated list of reviews for the course.",
        typeof(PaginatedReviewsDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course ID or pagination params.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_REVIEW")]
    public async Task<IActionResult> GetCourseReviews(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Received get reviews request for course {CourseId}, page {Page}, size {Size}.",
            courseId,
            page,
            pageSize
        );
        PaginatedReviewsDto result = await _mediator.Send(
            new GetCourseReviewsQuery(courseId, page, pageSize),
            cancellationToken
        );
        _logger.LogDebug(
            "Completed get reviews request for course {CourseId}.",
            courseId
        );
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a review by a specific user for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The review details.</returns>
    [HttpGet("course/{course-id}/user/{user-id}/review")]
    [SwaggerResponse(StatusCodes.Status200OK, "The review details.", typeof(ReviewDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid IDs.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Review or course not found.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_REVIEW")]
    public async Task<IActionResult> GetUserCourseReview(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromRoute(Name = "user-id")] Guid userId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received get user review request for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );
        ReviewDto result = await _mediator.Send(
            new GetUserCourseReviewQuery(courseId, userId),
            cancellationToken
        );
        _logger.LogDebug(
            "Finished get user review request for course {CourseId} and user {UserId}.",
            courseId,
            userId
        );
        return Ok(result);
    }
}
