using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Auth.Command;

/// <summary>
/// Command to log out a user by revoking their refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record LogoutCommand(string RefreshToken) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the logout request.
/// </summary>
public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutValidator"/> class.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutValidator"/> class.
    /// </summary>
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
    }
}

/// <summary>
/// Handles the logout process by revoking the provided refresh token.
/// </summary>
public class LogoutHandler : IRequestHandler<LogoutCommand, SuccessResponse>
{
    private readonly IAuthService _authService;
    private readonly ICustomLogger<LogoutHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutHandler"/> class.
    /// </summary>
    public LogoutHandler(IAuthService authService, ICustomLogger<LogoutHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the logout process by revoking the provided refresh token.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing logout request");

        RefreshToken? refreshToken = await _authService.GetRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
        );

        if (refreshToken is null)
        {
            return new SuccessResponse { Message = "Logged out successfully" };
        }

        await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        await _authService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo("Logout successful");

        return new SuccessResponse { Message = "Logged out successfully" };
    }
}
