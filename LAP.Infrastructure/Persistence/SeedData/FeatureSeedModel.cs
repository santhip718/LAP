namespace LAP.Infrastructure.Persistence.SeedData;

/// <summary>
/// Data transfer object used for seeding feature entities from configuration.
/// </summary>
public class FeatureSeedModel
{
    /// <summary>
    /// Gets or sets the name of the feature.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP method (e.g., GET, POST) for the feature endpoint.
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description of the feature.
    /// </summary>
    public string? Description { get; set; }
}