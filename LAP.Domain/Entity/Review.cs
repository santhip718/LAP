namespace LAP.Domain.Entity;

/// <summary>
/// Stores a user's rating and optional review text for a course.
/// </summary>
public class Review : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the reviewing user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the foreign key to the reviewed course.</summary>
    public Guid CourseId { get; set; }

    /// <summary>Gets or sets the numeric rating given by the user.</summary>
    public int Rating { get; set; }

    /// <summary>Gets or sets the optional review text.</summary>
    public string? ReviewText { get; set; }

    /// <summary>Gets or sets the associated user who wrote the review.</summary>
    public User User { get; set; } = null!;

    /// <summary>Gets or sets the associated course being reviewed.</summary>
    public Course Course { get; set; } = null!;
}
