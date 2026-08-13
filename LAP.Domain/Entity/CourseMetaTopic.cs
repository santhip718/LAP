namespace LAP.Domain.Entity;

/// <summary>
/// Represents a topic or module within a course, containing content items and questions.
/// </summary>
public class CourseMetaTopic : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent course.</summary>
    public Guid CourseId { get; set; }

    /// <summary>Gets or sets the display order of this topic within the course.</summary>
    public int SequenceOrder { get; set; }

    /// <summary>Gets or sets the name of the topic.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the duration of the topic in minutes.</summary>
    public int DurationMinute { get; set; }

    /// <summary>Gets or sets the parent course.</summary>
    public Course Course { get; set; } = null!;

    /// <summary>Gets or sets the collection of content items in this topic.</summary>
    public ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();

    /// <summary>Gets or sets the collection of questions associated with this topic.</summary>
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
