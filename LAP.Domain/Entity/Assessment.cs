namespace LAP.Domain.Entity;

/// <summary>
/// Represents an assessment or exam linked to a course with scoring and duration details.
/// </summary>
public class Assessment : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated course.</summary>
    public Guid CourseId { get; set; }

    /// <summary>Gets or sets the title of the assessment.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional description of the assessment.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the total marks for the assessment.</summary>
    public int TotalMark { get; set; }

    /// <summary>Gets or sets the minimum marks required to pass.</summary>
    public int PassingMark { get; set; }

    /// <summary>Gets or sets the duration of the assessment in minutes.</summary>
    public int DurationMinute { get; set; }

    /// <summary>Gets or sets the associated course.</summary>
    public Course Course { get; set; } = null!;

    /// <summary>Gets or sets the collection of questions in this assessment.</summary>
    public ICollection<Question> Questions { get; set; } = new List<Question>();

    /// <summary>Gets or sets the collection of assessment attempt history records.</summary>
    public ICollection<AssessmentHistory> AssessmentHistories { get; set; } =
        new List<AssessmentHistory>();

    /// <summary>Gets or sets the collection of import jobs for this assessment.</summary>
    public ICollection<ImportJob> ImportJobs { get; set; } = new List<ImportJob>();
}
