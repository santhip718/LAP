namespace LAP.Domain.Entity;

/// <summary>
/// Represents a discussion message posted by a user on a course forum.
/// </summary>
public class ForumMessage : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated course.</summary>
    public Guid CourseId { get; set; }

    /// <summary>Gets or sets the foreign key to the posting user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the message content.</summary>
    public string MessageText { get; set; } = string.Empty;

    /// <summary>Gets or sets the associated course.</summary>
    public Course Course { get; set; } = null!;

    /// <summary>Gets or sets the associated user who posted the message.</summary>
    public User User { get; set; } = null!;
}
