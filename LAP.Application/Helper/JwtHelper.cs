using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LAP.Application.DTO;
using LAP.Application.DTO.Auth;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LAP.Application.Helpers;

/// <summary>
/// Generates JWT access tokens and refresh tokens with user claims.
/// </summary>
public class JwtHelper : IJwtHelper
{
    private readonly JwtSettings _jwtSettings;
    private readonly ICustomLogger<JwtHelper> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtHelper"/> class.
    /// </summary>
    /// <param name="jwtSettings">The JWT configuration settings.</param>
    /// <param name="logger">The custom logger.</param>
    public JwtHelper(IOptions<JwtSettings> jwtSettings, ICustomLogger<JwtHelper> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generates an access token and refresh token for the specified user with role claims.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="roles">The list of roles assigned to the user.</param>
    /// <returns>An <see cref="AuthTokenResponseDto"/> containing the generated tokens and expiry.</returns>
    public AuthTokenResponseDto GenerateToken(
        Guid userId,
        string email,
        string fullName,
        List<string> roles
    )
    {
        _logger.LogDebug("Generating tokens for user {UserId} with email {Email}.", userId, email);
        SymmetricSecurityKey key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
        );
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, fullName),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: credentials
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        _logger.LogDebug("Generated tokens for user {UserId}.", userId);
        return new AuthTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpiryInMinutes * 60,
        };
    }

    /// <summary>
    /// Generates a new cryptographically secure random refresh token.
    /// </summary>
    /// <returns>A base64 encoded string representing the refresh token.</returns>
    public string GenerateRefreshToken()
    {
        _logger.LogDebug("Generating new refresh token.");
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _logger.LogDebug("Generated new refresh token.");
        return token;
    }
}
