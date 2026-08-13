namespace LAP.Infrastructure.Persistence.SeedData;

/// <summary>
/// Data transfer object used for seeding role-to-feature permission mappings from configuration.
/// </summary>
public class RoleFeatureMappingSeedModel
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the feature associated with the role.
    /// </summary>
    public string FeatureName { get; set; } = string.Empty;
}
