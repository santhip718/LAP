namespace LAP.Domain.Entity;

/// <summary>
/// Join entity linking a role to a feature for authorization and permission management.
/// </summary>
public class RoleFeatureMapping : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the role reference term.</summary>
    public Guid RoleId { get; set; }

    /// <summary>Gets or sets the foreign key to the feature.</summary>
    public Guid FeatureId { get; set; }

    /// <summary>Gets or sets the associated role reference term.</summary>
    public RefTerm Role { get; set; } = null!;

    /// <summary>Gets or sets the associated feature.</summary>
    public Feature Feature { get; set; } = null!;
}
