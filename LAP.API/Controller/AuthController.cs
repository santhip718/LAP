using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Application.Feature.Auth.Command;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAP.API.Controller;

/// <summary>
/// Handles authentication operations including user registration, login, token refresh and logout.
/// </summary>
[Route("api/v1/auth")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="logger">The custom logger.</param>
    public AuthController(IMediator mediator, ICustomLogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Registers a new user account and returns authentication tokens.</summary>
    /// <param name="dto">The registration details including name, email, password and mobile number.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the JWT access and refresh tokens.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received register request");

        AuthTokenResponseDto result = await _mediator.Send(
            new RegisterCommand(dto),
            cancellationToken
        );

        _logger.LogDebug("Register request completed successfully");
        return Ok(result);
    }

    /// <summary>Authenticates a user with email and password credentials.</summary>
    /// <param name="dto">The login credentials containing email and password.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the JWT access and refresh tokens.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received login request");

        AuthTokenResponseDto result = await _mediator.Send(
            new LoginCommand(dto),
            cancellationToken
        );
        _logger.LogDebug("Login request completed successfully");
        return Ok(result);
    }

    /// <summary>Generates a new access token using a valid refresh token.</summary>
    /// <param name="dto">The refresh request containing the refresh token.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new JWT access and refresh tokens.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received refresh token request");

        AuthTokenResponseDto result = await _mediator.Send(
            new RefreshTokenCommand(dto),
            cancellationToken
        );
        _logger.LogDebug("Refresh token request completed successfully");
        return Ok(result);
    }

    /// <summary>Revokes the specified refresh token to end the user session.</summary>
    /// <param name="dto">The refresh request containing the refresh token to revoke.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating whether the token was successfully revoked.</returns>
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received logout request");

        SuccessResponse result = await _mediator.Send(
            new LogoutCommand(dto.RefreshToken),
            cancellationToken
        );
        _logger.LogDebug("Logout request completed successfully");
        return Ok(result);
    }
}
