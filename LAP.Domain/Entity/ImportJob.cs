namespace LAP.Domain.Entity;

/// <summary>
/// Tracks the progress and status of bulk question import operations for an assessment.
/// </summary>
public class ImportJob : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated assessment.</summary>
    public Guid AssessmentId { get; set; }

    /// <summary>Gets or sets the foreign key to the import status reference term.</summary>
    public Guid StatusId { get; set; }

    /// <summary>Gets or sets the total number of records to import.</summary>
    public int TotalRecords { get; set; }

    /// <summary>Gets or sets the number of records successfully processed.</summary>
    public int ProcessedRecords { get; set; }

    /// <summary>Gets or sets the number of records that failed to import.</summary>
    public int FailedRecords { get; set; }

    /// <summary>Gets or sets the date and time when the import started.</summary>
    public DateTime StartedOn { get; set; }

    /// <summary>Gets or sets the date and time when the import completed.</summary>
    public DateTime? CompletedOn { get; set; }

    /// <summary>Gets or sets the error message if the import failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Gets or sets the associated assessment.</summary>
    public Assessment Assessment { get; set; } = null!;

    /// <summary>Gets or sets the import status reference term.</summary>
    public RefTerm Status { get; set; } = null!;
}
