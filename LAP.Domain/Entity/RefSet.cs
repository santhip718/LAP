namespace LAP.Domain.Entity;

/// <summary>
/// A reference set category that groups related reference terms (e.g., Gender, Designation, DifficultyLevel).
/// </summary>
public class RefSet : BaseEntity
{
    /// <summary>Gets or sets the display name of the reference set.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional description of the reference set.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the collection of reference terms belonging to this set.</summary>
    public ICollection<RefTerm> RefTerms { get; set; } = new List<RefTerm>();
}
