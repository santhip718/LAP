namespace LAP.Domain.Entity;

/// <summary>
/// Stores the password hash and salt for a user's authentication credentials.
/// </summary>
public class UserSecret : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the hashed password for authentication.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Gets or sets the salt used for password hashing.</summary>
    public string PasswordSalt { get; set; } = string.Empty;

    /// <summary>Gets or sets the associated user.</summary>
    public User User { get; set; } = null!;
}
