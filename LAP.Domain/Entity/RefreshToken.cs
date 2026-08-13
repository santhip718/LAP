namespace LAP.Domain.Entity;

/// <summary>
/// Stores refresh tokens issued to users.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the refresh token string.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Gets or sets the date and time when the token expires.</summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the token has been revoked.</summary>
    public bool IsRevoked { get; set; }

    /// <summary>Gets or sets the associated user.</summary>
    public User User { get; set; } = null!;
}
