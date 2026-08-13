using Asp.Versioning;
using LAP.API.Authorization;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.CourseContent;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Feature.CourseContent.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Handles course content management and operations, including adding, updating, deleting content,
/// as well as retrieval and completion tracking.
/// </summary>
[Route("api/v1/course-content")]
public class CourseContentController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<CourseContentController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CourseContentController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for dispatching commands and queries.</param>
    /// <param name="logger">The custom logger for structured logging.</param>
    public CourseContentController(
        IMediator mediator,
        ICustomLogger<CourseContentController> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves complete details of a course content item.
    /// </summary>
    /// <param name="id">The unique identifier of the course content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The course content details including meta topic and completion status.</returns>
    [HttpGet("{id}")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Course content details retrieved successfully.",
        typeof(CourseContentDetailDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "Insufficient permissions.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Course content not found.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_COURSE_CONTENT")]
    public async Task<IActionResult> GetCourseContentById(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received request for course content details: {ContentId}", id);
        CourseContentDetailDto result = await _mediator.Send(
            new GetCourseContentByIdQuery(id),
            cancellationToken
        );
        _logger.LogDebug("Completed request for course content details: {ContentId}", id);
        return Ok(result);
    }

    /// <summary>Adds new content to a course with the provided details.</summary>
    /// <param name="dto">The course content creation request containing course, content and file metadata.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the created course content identifier.</returns>
    [HttpPost]
    [Authorize]
    [FeatureAuthorize("MANAGE_COURSE_CONTENT")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Course content created successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> Add(
        [FromForm] CreateCourseContentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received add course content request for course {CourseId} and title {Title}.",
            dto.CourseId,
            dto.Title
        );

        SuccessResponse result = await _mediator.Send(
            new AddCourseContentCommand(dto),
            cancellationToken
        );

        _logger.LogDebug("Added course content {ContentId} for course {CourseId}.", result.Id);
        return CreatedAtAction(nameof(GetCourseContentById), new { id = result.Id }, result);
    }

    /// <summary>Updates an existing course content with the provided details.</summary>
    /// <param name="id">The identifier of the course content to update.</param>
    /// <param name="dto">The course content update request containing content and file metadata.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the updated course content identifier.</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [FeatureAuthorize("MANAGE_COURSE_CONTENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Course content updated successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course content not found.")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdateCourseContentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received update course content request for content {ContentId}.", id);

        SuccessResponse result = await _mediator.Send(
            new UpdateCourseContentCommand(id, dto),
            cancellationToken
        );

        _logger.LogDebug("Updated course content {ContentId} for course {CourseId}.", result.Id);
        return Ok(result);
    }

    /// <summary>Deletes an existing course content by its identifier.</summary>
    /// <param name="id">The identifier of the course content to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the deleted course content identifier.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [FeatureAuthorize("MANAGE_COURSE_CONTENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Course content deleted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course content not found.")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received delete course content request for content {ContentId}.", id);

        SuccessResponse result = await _mediator.Send(
            new DeleteCourseContentCommand(id),
            cancellationToken
        );

        _logger.LogDebug("Deleted course content {ContentId} for course {CourseId}.", result.Id);
        return Ok(result);
    }

    /// <summary>
    /// Marks a course content as completed or incomplete for the current user.
    /// </summary>
    /// <param name="id">The unique identifier of the course content.</param>
    /// <param name="request">The completion status details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Updated progress information.</returns>
    [HttpPut("{id}/completion-status")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Completion status updated successfully.",
        typeof(UpdateContentCompletionStatusResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid request or user not enrolled.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "Insufficient permissions.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Course content not found.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("UPDATE_COURSE_PROGRESS")]
    public async Task<IActionResult> UpdateCompletionStatus(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] UpdateContentCompletionStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received request to update completion status for content {ContentId}.",
            id
        );
        UpdateContentCompletionStatusResponse result = await _mediator.Send(
            new UpdateContentCompletionStatusCommand(id, request),
            cancellationToken
        );
        _logger.LogDebug(
            "Completed request to update completion status for content {ContentId}.",
            id
        );
        return Ok(result);
    }
}
