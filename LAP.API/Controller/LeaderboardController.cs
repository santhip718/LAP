using LAP.API.Authorization;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Leaderboard.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Controller for leaderboard-related operations.
/// </summary>
[Route("api/v1/leaderboard")]
public class LeaderboardController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<LeaderboardController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="logger">The logger instance.</param>
    public LeaderboardController(IMediator mediator, ICustomLogger<LeaderboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the overall platform leaderboard.
    /// </summary>
    /// <param name="pageSize">The number of items to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of leaderboard entries.</returns>
    [HttpGet("overall")]
    [FeatureAuthorize("VIEW_LEADERBOARD")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Overall platform leaderboard retrieved successfully.",
        typeof(List<LeaderboardDto>)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> GetOverallPlatformLeaderboard(
        [FromQuery] int pageSize = CommonConstants.DEFAULT_PAGE_SIZE,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "GetOverallPlatformLeaderboard started with pageSize: {PageSize}",
            pageSize
        );

        List<LeaderboardDto> result = await _mediator.Send(
            new GetOverallPlatformLeaderboardQuery(pageSize),
            cancellationToken
        );

        _logger.LogDebug("Successfully retrieved overall platform leaderboard");
        return Ok(result);
    }
}
