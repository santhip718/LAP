namespace LAP.Domain.Entity;

/// <summary>
/// Represents a platform user linked to a person, tier, secrets, roles, enrollments, and activity history.
/// </summary>
public class User : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the associated person.</summary>
    public Guid PersonId { get; set; }

    /// <summary>Gets or sets the foreign key to the current tier reference term.</summary>
    public Guid? CurrentTierId { get; set; }

    /// <summary>Gets or sets the overall score for the user, calculated from assessments and activities.</summary>
    public decimal OverallScore { get; set; }

    /// <summary>Gets or sets the associated person.</summary>
    public Person Person { get; set; } = null!;

    /// <summary>Gets or sets the current tier reference term.</summary>
    public RefTerm? CurrentTier { get; set; }

    /// <summary>Gets or sets the user secret for authentication.</summary>
    public UserSecret UserSecret { get; set; } = null!;

    /// <summary>Gets or sets the collection of role mappings assigned to this user.</summary>
    public ICollection<UserRoleMapping> UserRoles { get; set; } = new List<UserRoleMapping>();

    /// <summary>Gets or sets the collection of course enrollments for this user.</summary>
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    /// <summary>Gets or sets the collection of assessment attempt history records.</summary>
    public ICollection<AssessmentHistory> AssessmentHistories { get; set; } =
        new List<AssessmentHistory>();

    /// <summary>Gets or sets the collection of reviews written by this user.</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Gets or sets the collection of forum messages posted by this user.</summary>
    public ICollection<ForumMessage> ForumMessages { get; set; } = new List<ForumMessage>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>Gets or sets the overall weighted score for the user across the platform.</summary>
    public decimal OverallWeightedScore { get; set; }
}
