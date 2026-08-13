namespace LAP.Infrastructure.Persistence.SeedData;

/// <summary>
/// Data transfer object used for seeding reference set entities from configuration.
/// </summary>
public class RefSetSeedModel
{
    /// <summary>
    /// Gets or sets the name of the reference set.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description of the reference set.
    /// </summary>
    public string? Description { get; set; }
}
