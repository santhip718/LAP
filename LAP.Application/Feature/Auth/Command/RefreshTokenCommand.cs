using FluentValidation;
using LAP.Application.DTO;
using LAP.Application.DTO.Auth;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LAP.Application.Feature.Auth.Command;

/// <summary>Command to refresh an expired JWT access token using a valid refresh token.</summary>
/// <param name="Dto">The refresh request containing the refresh token.</param>
public record RefreshTokenCommand(RefreshRequestDto Dto) : IRequest<AuthTokenResponseDto>;

/// <summary>Validates <see cref="RefreshTokenCommand"/> rules before processing.</summary>
public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenValidator"/> class.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenValidator"/> class.
    /// </summary>
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Dto.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
    }
}

/// <summary>
/// Handles the generation of a new access token using a valid refresh token.
/// </summary>
public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthTokenResponseDto>
{
    private readonly IAuthService _authService;
    private readonly IJwtHelper _jwtHelper;
    private readonly ICustomLogger<RefreshTokenHandler> _logger;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class.
    /// </summary>
    public RefreshTokenHandler(
        IAuthService authService,
        IJwtHelper jwtHelper,
        ICustomLogger<RefreshTokenHandler> logger,
        IOptions<JwtSettings> jwtSettings
    )
    {
        _authService = authService;
        _jwtHelper = jwtHelper;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Handles the refresh token request.
    /// </summary>
    public async Task<AuthTokenResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing refresh token request");

        string tokenValue = request.Dto.RefreshToken?.Trim() ?? string.Empty;

        RefreshToken? refreshToken = await _authService.GetRefreshTokenAsync(
            tokenValue,
            cancellationToken
        );

        if (
            refreshToken is null
            || !refreshToken.IsActive
            || refreshToken.IsRevoked
            || refreshToken.ExpiryDate < DateTime.UtcNow
        )
        {
            _logger.LogError("Invalid refresh token");

            throw new UnauthorizedException(
                "Invalid refresh token",
                "Refresh token is invalid or expired."
            );
        }

        Domain.Entity.User user = refreshToken.User;

        if (user is null)
        {
            _logger.LogError("Refresh token failed. User not found for token.");
            throw new UnauthorizedException("Invalid refresh token", "User not found.");
        }

        List<string> roles = user
            .UserRoles.Select(x => x.Role?.Name)
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>()
            .ToList();

        AuthTokenResponseDto response = _jwtHelper.GenerateToken(
            user.Id,
            user.Person.Email,
            user.Person.FullName,
            roles
        );

        // Revoke the old token
        await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        // Save the new refresh token
        int expiryDays =
            _jwtSettings.RefreshTokenExpiryInDays > 0 ? _jwtSettings.RefreshTokenExpiryInDays : 7;

        RefreshToken newRefreshToken = new()
        {
            UserId = user.Id,
            Token = response.RefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false,
        };

        await _authService.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _authService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo("Refresh token successful for user {UserId}", user.Id);

        return response;
    }
}
