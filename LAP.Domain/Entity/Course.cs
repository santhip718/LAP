namespace LAP.Domain.Entity;

/// <summary>
/// Core course entity with title, description, categorization, difficulty, rating, and related collections.
/// </summary>
public class Course : BaseEntity
{
    /// <summary>Gets or sets the title of the course.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional description of the course.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the foreign key to the category reference term.</summary>
    public Guid CategoryId { get; set; }

    /// <summary>Gets or sets the foreign key to the sub-category reference term.</summary>
    public Guid SubCategoryId { get; set; }

    /// <summary>Gets or sets the overall rating calculated from user reviews.</summary>
    public decimal OverallRating { get; set; }

    /// <summary>Gets or sets the foreign key to the difficulty level reference term.</summary>
    public Guid DifficultyLevelId { get; set; }

    /// <summary>Gets or sets the foreign key to the user who created the course.</summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>Gets or sets the total duration of the course in minutes.</summary>
    public int DurationMinute { get; set; }

    /// <summary>Gets or sets the file path for the course thumbnail image.</summary>
    public string? ThumbnailImgPath { get; set; }

    /// <summary>Gets or sets a value indicating whether the course is drafted.</summary>
    public bool IsDrafted { get; set; }

    /// <summary>Gets or sets the category reference term.</summary>
    public RefTerm Category { get; set; } = null!;

    /// <summary>Gets or sets the sub-category reference term.</summary>
    public RefTerm SubCategory { get; set; } = null!;

    /// <summary>Gets or sets the difficulty level reference term.</summary>
    public RefTerm DifficultyLevel { get; set; } = null!;

    /// <summary>Gets or sets the user who created the course.</summary>
    public User CreatedByUser { get; set; } = null!;

    /// <summary>Gets or sets the collection of topics in this course.</summary>
    public ICollection<CourseMetaTopic> Topics { get; set; } = new List<CourseMetaTopic>();

    /// <summary>Gets or sets the collection of enrollments for this course.</summary>
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    /// <summary>Gets or sets the collection of reviews for this course.</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Gets or sets the collection of forum messages for this course.</summary>
    public ICollection<ForumMessage> ForumMessages { get; set; } = new List<ForumMessage>();

    /// <summary>Gets or sets the associated assessment for this course.</summary>
    public Assessment? Assessment { get; set; }
}
