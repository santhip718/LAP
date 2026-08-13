namespace LAP.Domain.Entity;

/// <summary>
/// Records a user's attempt at an assessment, storing scores, timing, and tier awarded.
/// </summary>
public class AssessmentHistory : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the user who took the assessment.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the foreign key to the assessment.</summary>
    public Guid AssessmentId { get; set; }

    /// <summary>Gets or sets the date and time when the assessment was started.</summary>
    public DateTime StartedOn { get; set; }

    /// <summary>Gets or sets the date and time when the assessment was completed.</summary>
    public DateTime? CompletedOn { get; set; }

    /// <summary>Gets or sets the raw score achieved by the user.</summary>
    public decimal Score { get; set; }

    /// <summary>Gets or sets the weighted score after applying question weights.</summary>
    public decimal WeightedScore { get; set; }

    /// <summary>Gets or sets the foreign key to the tier awarded based on performance.</summary>
    public Guid? TierAwardedId { get; set; }

    /// <summary>Gets or sets the associated user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Gets or sets the associated assessment.</summary>
    public Assessment Assessment { get; set; } = null!;

    /// <summary>Gets or sets the tier reference term awarded to the user.</summary>
    public RefTerm? TierAwarded { get; set; }

    /// <summary>Gets or sets the collection of answers submitted for this attempt.</summary>
    public ICollection<AssessmentAnswer> Answers { get; set; } = new List<AssessmentAnswer>();
}
