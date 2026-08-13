namespace LAP.Infrastructure.Persistence.SeedData;

/// <summary>
/// Data transfer object used for seeding reference term entities from configuration.
/// </summary>
public class RefTermSeedModel
{
    /// <summary>
    /// Gets or sets the name of the parent reference set this term belongs to.
    /// </summary>
    public string RefSetName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the reference term.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description of the reference term.
    /// </summary>
    public string? Description { get; set; }
}
