using LAP.API.Authorization;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Controller for managing assessment and their associated question.
/// </summary>
[Route("api/v1/assessment")]
public class AssessmentController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<AssessmentController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AssessmentController(IMediator mediator, ICustomLogger<AssessmentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new assessment along with its question from an uploaded Excel file.
    /// </summary>
    /// <param name="command">The assessment details and question file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The unique identifier of the created assessment.</returns>
    [HttpPost]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [Consumes("multipart/form-data")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Assessment created successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> CreateAssessment(
        [FromForm] CreateAssessmentCommand command,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received assessment creation request");

        SuccessResponse result = await _mediator.Send(command, cancellationToken);

        _logger.LogDebug("Assessment creation request completed successfully");
        return CreatedAtAction(nameof(GetQuestionByAssessmentId), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves all assessment with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number (default 1).</param>
    /// <param name="pageSize">The number of records per page (default 25).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of assessments.</returns>
    [HttpGet]
    [FeatureAuthorize("VIEW_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Assessment retrieved successfully.",
        typeof(PaginatedAssessmentsDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> GetAllAssessment(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Received get all assessment request (page {PageNumber}, size {PageSize})",
            pageNumber,
            pageSize
        );

        PaginatedAssessmentsDto result = await _mediator.Send(
            new GetAllAssessmentQuery(pageNumber, pageSize),
            cancellationToken
        );

        _logger.LogDebug("Get all assessment request completed successfully");
        return Ok(result);
    }

    /// <summary>
    /// Submits assessment answers for evaluation and returns the result.
    /// </summary>
    /// <param name="id">The unique identifier of the assessment.</param>
    /// <param name="request">The submission payload containing user answers.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The assessment submission result including score and pass/fail status.</returns>
    [HttpPost("{id}/submit")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The assessment submission result.",
        typeof(SubmitAssessmentResponseDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid submission data or user is not enrolled.",
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
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assessment not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("SUBMIT_ASSESSMENT")]
    public async Task<IActionResult> SubmitAssessment(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] AssessmentSubmitRequestDto request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received assessment submission request for assessment {AssessmentId}.",
            id
        );
        SubmitAssessmentResponseDto result = await _mediator.Send(
            new SubmitAssessmentCommand(id, request),
            cancellationToken
        );
        _logger.LogDebug(
            "Finished assessment submission request for assessment {AssessmentId}.",
            id
        );
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all question associated with a specific assessment.
    /// </summary>
    /// <param name="id">The unique identifier of the assessment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of question for the assessment.</returns>
    [HttpGet("{id}/question")]
    [FeatureAuthorize("VIEW_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Assessment question retrieved successfully.",
        typeof(List<QuestionDto>)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assessment not found.")]
    public async Task<IActionResult> GetQuestionByAssessmentId(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received get question request for assessment: {AssessmentId}", id);

        List<QuestionDto> question = await _mediator.Send(
            new GetQuestionByAssessmentIdQuery(id),
            cancellationToken
        );

        _logger.LogDebug(
            "Get question request completed successfully for assessment: {AssessmentId}",
            id
        );
        return Ok(question);
    }

    /// <summary>
    /// Updates an existing assessment by its unique identifier.
    /// </summary>
    /// <param name="id">The assessment identifier.</param>
    /// <param name="dto">The updated assessment details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A boolean indicating success.</returns>
    [HttpPut("{id}")]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Assessment updated successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assessment not found.")]
    public async Task<IActionResult> UpdateAssessmentById(
        Guid id,
        [FromBody] UpdateAssessmentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received update assessment request for assessment: {AssessmentId}", id);

        SuccessResponse result = await _mediator.Send(
            new UpdateAssessmentByIdCommand(id, dto),
            cancellationToken
        );

        _logger.LogDebug(
            "Update assessment request completed successfully for assessment: {AssessmentId}",
            id
        );
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the latest assessment result for a specific assessment.
    /// </summary>
    /// <param name="id">The unique identifier of the assessment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The latest assessment result with score details and pass/fail status.</returns>
    [HttpGet("{id}/result")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The latest assessment result.",
        typeof(AssessmentResultResponseDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid assessment ID.",
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
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Assessment or result not found.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_ASSESSMENT")]
    public async Task<IActionResult> GetAssessmentResult(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received assessment result request for assessment {AssessmentId}.", id);
        AssessmentResultResponseDto result = await _mediator.Send(
            new GetAssessmentResultQuery(id),
            cancellationToken
        );
        _logger.LogDebug("Finished assessment result request for assessment {AssessmentId}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an assessment by its unique identifier.
    /// </summary>
    /// <param name="id">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A boolean indicating success.</returns>
    [HttpDelete("{id}")]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Assessment deleted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assessment not found.")]
    public async Task<IActionResult> DeleteAssessmentById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received delete assessment request for assessment: {AssessmentId}", id);

        SuccessResponse result = await _mediator.Send(
            new DeleteAssessmentByIdCommand(id),
            cancellationToken
        );

        _logger.LogDebug(
            "Delete assessment request completed successfully for assessment: {AssessmentId}",
            id
        );
        return Ok(result);
    }

    /// <summary>
    /// Updates a specific question by its unique identifier.
    /// </summary>
    /// <param name="id">The question identifier.</param>
    /// <param name="dto">The updated question details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A boolean indicating success.</returns>
    [HttpPut("question/{id}")]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Question updated successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Question not found.")]
    public async Task<IActionResult> UpdateQuestionById(
        Guid id,
        [FromBody] UpdateQuestionRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received update question request for question: {QuestionId}", id);

        SuccessResponse result = await _mediator.Send(
            new UpdateQuestionByIdCommand(id, dto),
            cancellationToken
        );

        _logger.LogDebug(
            "Update question request completed successfully for question: {QuestionId}",
            id
        );
        return Ok(result);
    }

    /// <summary>
    /// Deletes a specific question by its unique identifier.
    /// </summary>
    /// <param name="id">The question identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A boolean indicating success.</returns>
    [HttpDelete("question/{id}")]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Question deleted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Question not found.")]
    public async Task<IActionResult> DeleteQuestionById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received delete question request for question: {QuestionId}", id);

        SuccessResponse result = await _mediator.Send(
            new DeleteQuestionByIdCommand(id),
            cancellationToken
        );

        _logger.LogDebug(
            "Delete question request completed successfully for question: {QuestionId}",
            id
        );
        return Ok(result);
    }

    /// <summary>
    /// Exports the question import template as an Excel file.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The question template file.</returns>
    [HttpGet("export-template")]
    [FeatureAuthorize("MANAGE_ASSESSMENT")]
    [SwaggerResponse(StatusCodes.Status200OK, "Template exported successfully.")]
    public async Task<IActionResult> ExportTemplate(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received export template request");

        (byte[] fileContents, string contentType, string fileName) = await _mediator.Send(
            new ExportTemplateQuery(),
            cancellationToken
        );

        _logger.LogDebug("Export template request completed successfully");
        return File(fileContents, contentType, fileName);
    }

    /// <summary>
    /// Retrieves the complete assessment attempt history for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="pageNumber">The page number (default 1).</param>
    /// <param name="pageSize">The number of records per page (default 10).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of assessment history records.</returns>
    [HttpGet("user/{user-id}/assessment-history")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A paginated list of assessment history records.",
        typeof(PaginatedAssessmentHistoryResponseDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid user ID or pagination parameters.",
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
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_ASSESSMENT_HISTORY")]
    public async Task<IActionResult> GetUserAssessmentHistory(
        [FromRoute(Name = "user-id")] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Received assessment history request for user {UserId} (page {PageNumber}, size {PageSize}).",
            userId,
            pageNumber,
            pageSize
        );
        PaginatedAssessmentHistoryResponseDto result = await _mediator.Send(
            new GetUserAssessmentHistoryQuery(userId, pageNumber, pageSize),
            cancellationToken
        );
        _logger.LogDebug("Finished assessment history request for user {UserId}.", userId);
        return Ok(result);
    }
}
