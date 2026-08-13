namespace LAP.Domain.Entity;

/// <summary>
/// Stores a user's answer to a specific question within an assessment attempt.
/// </summary>
public class AssessmentAnswer : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent assessment attempt.</summary>
    public Guid AssessmentHistoryId { get; set; }

    /// <summary>Gets or sets the foreign key to the answered question.</summary>
    public Guid QuestionId { get; set; }

    /// <summary>Gets or sets the answer selected by the user.</summary>
    public string SelectedAnswer { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the selected answer is correct.</summary>
    public bool IsCorrect { get; set; }

    /// <summary>Gets or sets the score obtained for this answer.</summary>
    public decimal ObtainedScore { get; set; }

    /// <summary>Gets or sets the parent assessment attempt.</summary>
    public AssessmentHistory AssessmentHistory { get; set; } = null!;

    /// <summary>Gets or sets the associated question.</summary>
    public Question Question { get; set; } = null!;
}
