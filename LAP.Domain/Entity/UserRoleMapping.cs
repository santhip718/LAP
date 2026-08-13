namespace LAP.Domain.Entity;

/// <summary>
/// Join entity linking a user to a role for role-based authorization.
/// </summary>
public class UserRoleMapping : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the foreign key to the role reference term.</summary>
    public Guid RoleId { get; set; }

    /// <summary>Gets or sets the associated user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Gets or sets the role reference term.</summary>
    public RefTerm Role { get; set; } = null!;
}
