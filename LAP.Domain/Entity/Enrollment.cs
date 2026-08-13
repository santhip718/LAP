namespace LAP.Domain.Entity;

/// <summary>
/// Tracks a user's enrollment in a course including progress, dates, and status.
/// </summary>
public class Enrollment : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the enrolled user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the foreign key to the enrolled course.</summary>
    public Guid CourseId { get; set; }

    /// <summary>Gets or sets the date and time when the user enrolled.</summary>
    public DateTime EnrolledOn { get; set; }

    /// <summary>Gets or sets the date and time when the course was completed.</summary>
    public DateTime? CompletedOn { get; set; }

    /// <summary>Gets or sets the overall progress percentage for this enrollment.</summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>Gets or sets a value indicating whether the enrollment is active or completed.</summary>
    public bool EnrollmentStatus { get; set; }

    /// <summary>Gets or sets the associated user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Gets or sets the associated course.</summary>
    public Course Course { get; set; } = null!;

    /// <summary>Gets or sets the collection of individual content progress items.</summary>
    public ICollection<UserCourseProgress> ProgressItems { get; set; } =
        new List<UserCourseProgress>();
}
