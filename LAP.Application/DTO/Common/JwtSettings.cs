namespace LAP.Application.DTO;

/// <summary>
/// Represents the configuration settings for JWT authentication.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the secret key used to sign JWT tokens.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issuer of the JWT token.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience for the JWT token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiry duration in minutes.
    /// </summary>
    public int ExpiryInMinutes { get; set; }

    /// <summary>
    /// Gets or sets the refresh token expiry duration in days.
    /// </summary>
    public int RefreshTokenExpiryInDays { get; set; }
}
