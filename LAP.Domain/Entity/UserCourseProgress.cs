namespace LAP.Domain.Entity;

/// <summary>
/// Tracks a user's completion status for individual course content items within an enrollment.
/// </summary>
public class UserCourseProgress : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the enrollment.</summary>
    public Guid EnrollmentId { get; set; }

    /// <summary>Gets or sets the foreign key to the course content item.</summary>
    public Guid CourseContentId { get; set; }

    /// <summary>Gets or sets a value indicating whether the content item is completed.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Gets or sets the date and time when the content was completed.</summary>
    public DateTime? CompletedOn { get; set; }

    /// <summary>Gets or sets the associated enrollment.</summary>
    public Enrollment Enrollment { get; set; } = null!;

    /// <summary>Gets or sets the associated course content.</summary>
    public CourseContent CourseContent { get; set; } = null!;
}
