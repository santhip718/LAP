using System.Text.RegularExpressions;
using FluentValidation;
using LAP.Application.Constant;
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

/// <summary>
/// Command for registering a new user.
/// </summary>
/// <param name="Dto">The registration request data.</param>
public record RegisterCommand(RegisterRequestDto Dto) : IRequest<AuthTokenResponseDto>;

/// <summary>
/// Validator for <see cref="RegisterCommand"/>.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private const string PasswordPattern =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterValidator"/> class.
    /// </summary>
    /// <param name="authService">The authentication service for uniqueness checks.</param>
    public RegisterValidator(IAuthService authService)
    {
        RuleFor(x => x.Dto.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Full name is required and cannot exceed 100 characters");

        RuleFor(x => x.Dto.Email)
            .NotEmpty()
            .WithMessage("Email address is required")
            .Matches(EmailPattern)
            .WithMessage(
                "Email address is invalid. It must follow a valid format (e.g., user@example.com)"
            )
            .MustAsync(
                async (email, cancellationToken) =>
                    !await authService.EmailExistsAsync(email, cancellationToken)
            )
            .WithMessage("Email already exists. A user with this email is already registered");

        RuleFor(x => x.Dto.MobileNumber).NotEmpty().WithMessage("Mobile number is required");

        RuleFor(x => x.Dto.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(PasswordPattern)
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"
            );
    }
}

/// <summary>
/// Handler for <see cref="RegisterCommand"/>.
/// </summary>
public class RegisterHandler : IRequestHandler<RegisterCommand, AuthTokenResponseDto>
{
    private static readonly Guid DefaultRoleId = RoleConstants.STUDENT_ID;

    private readonly IAuthService _authService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<RegisterHandler> _logger;
    private readonly IJwtHelper _jwtHelper;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="jwtHelper">The JWT helper.</param>
    /// <param name="jwtSettings">The JWT settings.</param>
    public RegisterHandler(
        IAuthService authService,
        ITransactionService transactionService,
        ICustomLogger<RegisterHandler> logger,
        IJwtHelper jwtHelper,
        IOptions<JwtSettings> jwtSettings
    )
    {
        _authService = authService;
        _transactionService = transactionService;
        _logger = logger;
        _jwtHelper = jwtHelper;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Handles the <see cref="RegisterCommand"/>.
    /// </summary>
    /// <param name="request">The register command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication token response.</returns>
    public async Task<AuthTokenResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        RegisterRequestDto dto = request.Dto;

        _logger.LogInfo(
            "Processing registration request for email: {Email}",
            PrivacyMaskHelper.MaskEmail(dto.Email)
        );

        string hash = UserSecretHelper.HashPasswordBcrypt(dto.Password, out string salt);

        string? roleName = await _authService.GetRoleNameByIdAsync(
            DefaultRoleId,
            cancellationToken
        );

        List<string> role = string.IsNullOrWhiteSpace(roleName)
            ? new List<string>()
            : new List<string> { roleName };

        return await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                Person person = new()
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    MobileNumber = dto.MobileNumber,
                    DesignationId = dto.DesignationId,
                    GenderId = dto.GenderId,
                };

                await _authService.AddPersonAsync(person, cancellationToken);

                Domain.Entity.User user = new() { PersonId = person.Id };

                await _authService.AddUserAsync(user, cancellationToken);

                UserSecret userSecret = new()
                {
                    UserId = user.Id,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                };

                await _authService.AddUserSecretAsync(userSecret, cancellationToken);

                UserRoleMapping userRoleMapping = new()
                {
                    UserId = user.Id,
                    RoleId = DefaultRoleId,
                };

                await _authService.AddUserRoleMappingAsync(userRoleMapping, cancellationToken);
                await _transactionService.SaveChangesAsync(cancellationToken);

                AuthTokenResponseDto response = _jwtHelper.GenerateToken(
                    user.Id,
                    person.Email,
                    person.FullName,
                    role
                );

                RefreshToken refreshToken = new()
                {
                    UserId = user.Id,
                    Token = response.RefreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays),
                    IsRevoked = false,
                };

                await _authService.AddRefreshTokenAsync(refreshToken, cancellationToken);
                await _transactionService.SaveChangesAsync(cancellationToken);

                _logger.LogInfo(
                    "Registration completed successfully for email: {Email}",
                    PrivacyMaskHelper.MaskEmail(dto.Email)
                );

                return response;
            },
            cancellationToken
        );
    }
}
