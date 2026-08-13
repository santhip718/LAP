using LAP.API.Authorization;
using LAP.Application.DTO.Common;
using LAP.Application.Feature.ReferenceData.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Provides reference data lookups by reference set name.
/// </summary>
[Route("api/v1/reference-data")]
public class ReferenceDataController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<ReferenceDataController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceDataController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator for dispatching queries.</param>
    /// <param name="logger">Application logger.</param>
    public ReferenceDataController(
        IMediator mediator,
        ICustomLogger<ReferenceDataController> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves reference terms for the specified reference set name.
    /// </summary>
    /// <param name="refSetName">The reference set name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of reference terms.</returns>
    [HttpGet("{ref-set-name}")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Reference data retrieved successfully.",
        typeof(List<RefTermDto>)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid reference set name.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Reference data not found.")]
    public async Task<IActionResult> GetReferenceData(
        [FromRoute(Name = "ref-set-name")] string refSetName,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received reference data request for {RefSetName}", refSetName);

        List<RefTermDto> result = await _mediator.Send(
            new GetReferenceDataQuery(refSetName),
            cancellationToken
        );

        _logger.LogDebug(
            "Reference data request completed successfully for {RefSetName}",
            refSetName
        );

        return Ok(result);
    }
}
