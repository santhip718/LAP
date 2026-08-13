using LAP.API.Authorization;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Enrollment;
using LAP.Application.DTO.Paginated;
using LAP.Application.Feature.Enrollment.Command;
using LAP.Application.Feature.Enrollment.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Handles enrollment management operations, including retrieval and status updates.
/// </summary>
[Authorize]
[Route("api/v1/enrollment")]
public class EnrollmentController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<EnrollmentController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for dispatching commands and queries.</param>
    /// <param name="logger">The custom application logger.</param>
    public EnrollmentController(IMediator mediator, ICustomLogger<EnrollmentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Updates an enrollment's status (accept or reject).
    /// </summary>
    /// <param name="id">The unique identifier of the enrollment to update.</param>
    /// <param name="dto">The request body containing the new enrollment status.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A success response containing the updated enrollment identifier.</returns>
    [HttpPut("{id:guid}")]
    [FeatureAuthorize("MANAGE_ENROLLMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Enrollment updated successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Enrollment not found.")]
    public async Task<IActionResult> UpdateEnrollment(
        [FromRoute] Guid id,
        [FromBody] UpdateEnrollmentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Updating enrollment {EnrollmentId} to status {Status}.",
            id,
            dto.EnrollmentStatus
        );

        SuccessResponse result = await _mediator.Send(
            new UpdateEnrollmentCommand(id, dto),
            cancellationToken
        );

        _logger.LogDebug("Updated enrollment {EnrollmentId}.", result.Id);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all enrollments with optional filters and pagination.
    /// </summary>
    /// <param name="courseName">An optional course name filter to narrow results.</param>
    /// <param name="categoryId">An optional course category identifier filter to narrow results.</param>
    /// <param name="userId">An optional user identifier filter to narrow results.</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of enrollments to include per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of enrollment details matching the specified filters.</returns>
    [HttpGet]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Enrollments retrieved successfully.",
        typeof(PaginatedEnrollmentsDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    public async Task<IActionResult> GetEnrollment(
        [FromQuery] string? courseName,
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching enrollments with course name {CourseName}, category {CategoryId}, page {Page}, and page size {PageSize}.",
            courseName,
            categoryId,
            page,
            pageSize
        );

        PaginatedEnrollmentsDto result = await _mediator.Send(
            new GetEnrollmentQuery(courseName, categoryId, page, pageSize),
            cancellationToken
        );

        _logger.LogDebug("Retrieved {Total} enrollments.", result.Total);

        return Ok(result);
    }
}
