using FluentValidation;
using LAP.Application.DTO;
using LAP.Application.DTO.Auth;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using LAP.Shared.Helpers;
using MediatR;
using Microsoft.Extensions.Options;

namespace LAP.Application.Feature.Auth.Command;

/// <summary>Command to authenticate a user with email and password.</summary>
/// <param name="Dto">The login credentials.</param>
public record LoginCommand(LoginRequestDto Dto) : IRequest<AuthTokenResponseDto>;

/// <summary>Validates <see cref="LoginCommand"/> rules before processing.</summary>
public class LoginValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginValidator"/> class.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginValidator"/> class.
    /// </summary>
    public LoginValidator()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required");

        RuleFor(x => x.Dto.Password).NotEmpty().WithMessage("Password is required");
    }
}

/// <summary>
/// Handles the login process, verifying credentials and generating an access token.
/// </summary>
public class LoginHandler : IRequestHandler<LoginCommand, AuthTokenResponseDto>
{
    private readonly IAuthService _authService;
    private readonly ICustomLogger<LoginHandler> _logger;
    private readonly IJwtHelper _jwtHelper;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginHandler"/> class.
    /// </summary>
    public LoginHandler(
        IAuthService authService,
        ICustomLogger<LoginHandler> logger,
        IJwtHelper jwtHelper,
        IOptions<JwtSettings> jwtSettings
    )
    {
        _authService = authService;
        _logger = logger;
        _jwtHelper = jwtHelper;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Handles the login request.
    /// </summary>
    public async Task<AuthTokenResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        LoginRequestDto dto = request.Dto;

        _logger.LogInfo(
            "Processing login request for email: {Email}",
            PrivacyMaskHelper.MaskEmail(dto.Email)
        );

        Domain.Entity.User? user = await _authService.GetUserByEmailAsync(
            dto.Email,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogError(
                "Login failed. User not found for email: {Email}",
                PrivacyMaskHelper.MaskEmail(dto.Email)
            );

            throw new UnauthorizedException(
                "Invalid credentials",
                "No account found with this email address."
            );
        }

        bool isValid = UserSecretHelper.VerifyPassword(dto.Password, user.UserSecret.PasswordHash);

        if (!isValid)
        {
            _logger.LogError(
                "Login failed. Invalid password for email: {Email}",
                PrivacyMaskHelper.MaskEmail(dto.Email)
            );

            throw new UnauthorizedException(
                "Invalid credentials",
                "The password you entered is incorrect."
            );
        }

        List<string> roles = user.UserRoles.Select(x => x.Role?.Name ?? string.Empty).ToList();

        AuthTokenResponseDto response = _jwtHelper.GenerateToken(
            user.Id,
            user.Person.Email,
            user.Person.FullName,
            roles
        );

        RefreshToken refreshToken = new()
        {
            UserId = user.Id,
            Token = response.RefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays),
            IsRevoked = false,
        };

        await _authService.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _authService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo(
            "Login completed successfully for email: {Email}",
            PrivacyMaskHelper.MaskEmail(dto.Email)
        );

        return response;
    }
}
