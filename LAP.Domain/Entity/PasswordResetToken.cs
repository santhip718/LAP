using LAP.Domain.Entity;

namespace LAP.Domain.Entity;

/// <summary>
/// Represents a password reset token issued to a user.
/// </summary>
public class PasswordResetToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier associated with the reset token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the hashed reset token value.
    /// The raw token should never be stored in the database.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    public DateTime ExpiresOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has already been used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the token was used.
    /// </summary>
    public DateTime? UsedOn { get; set; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
