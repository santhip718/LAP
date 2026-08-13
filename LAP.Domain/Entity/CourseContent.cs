namespace LAP.Domain.Entity;

/// <summary>
/// Represents an individual content item (video, PDF, etc.) within a course topic.
/// </summary>
public class CourseContent : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent meta topic.</summary>
    public Guid MetaTopicId { get; set; }

    /// <summary>Gets or sets the title of the content item.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the foreign key to the content type reference term.</summary>
    public Guid ContentTypeId { get; set; }

    /// <summary>Gets or sets the URL for video content.</summary>
    public string? VideoUrl { get; set; }

    /// <summary>Gets or sets the file path for PDF content.</summary>
    public string? PdfFilePath { get; set; }

    /// <summary>Gets or sets the display order within the topic.</summary>
    public int SequenceOrder { get; set; }

    /// <summary>Gets or sets the parent meta topic.</summary>
    public CourseMetaTopic MetaTopic { get; set; } = null!;

    /// <summary>Gets or sets the content type reference term.</summary>
    public RefTerm ContentType { get; set; } = null!;

    /// <summary>Gets or sets the collection of user progress records for this content.</summary>
    public ICollection<UserCourseProgress> UserCourseProgresses { get; set; } =
        new List<UserCourseProgress>();
}
