namespace LAP.Domain.Entity;

/// <summary>
/// Abstract base class providing common audit fields and soft-delete flag for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Gets or sets the unique identifier for the entity.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the identifier of the user who created this entity.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Gets or sets the date and time when this entity was created.</summary>
    public DateTime DateCreated { get; set; }

    /// <summary>Gets or sets the identifier of the user who last updated this entity.</summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>Gets or sets the date and time when this entity was last updated.</summary>
    public DateTime? DateUpdated { get; set; }

    /// <summary>Gets or sets a value indicating whether this entity is active (soft-delete flag).</summary>
    public bool IsActive { get; set; } = true;
}
