namespace LAP.Domain.Entity;

/// <summary>
/// Defines a securable feature with name and HTTP method for role-based authorization.
/// </summary>
public class Feature : BaseEntity
{
    /// <summary>Gets or sets the name of the feature.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the HTTP method (GET, POST, etc.) for the feature.</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional description of the feature.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the collection of role-feature mappings for this feature.</summary>
    public ICollection<RoleFeatureMapping> RoleFeatures { get; set; } =
        new List<RoleFeatureMapping>();
}
