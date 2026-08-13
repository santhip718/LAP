namespace LAP.Domain.Entity;

/// <summary>
/// A reference or lookup term belonging to a reference set, used for dropdown values throughout the system.
/// </summary>
public class RefTerm : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent reference set.</summary>
    public Guid RefSetId { get; set; }

    /// <summary>Gets or sets the display name of the reference term.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional description of the reference term.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the parent reference set.</summary>
    public RefSet RefSet { get; set; } = null!;
}
