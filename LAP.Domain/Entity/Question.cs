namespace LAP.Domain.Entity;

/// <summary>
/// Represents a question within an assessment with text, options, answer, and weight.
/// </summary>
public class Question : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent assessment.</summary>
    public Guid AssessmentId { get; set; }

    /// <summary>Gets or sets the foreign key to the associated meta topic.</summary>
    public Guid MetaTopicId { get; set; }

    /// <summary>Gets or sets the foreign key to the question type reference term.</summary>
    public Guid QuestionTypeId { get; set; }

    /// <summary>Gets or sets the question text.</summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>Gets or sets the list of answer options for this question.</summary>
    public List<string> OptionList { get; set; } = new();

    /// <summary>Gets or sets the correct answer for this question.</summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>Gets or sets the weight or point value of this question.</summary>
    public int Weight { get; set; }

    /// <summary>Gets or sets the parent assessment.</summary>
    public Assessment Assessment { get; set; } = null!;

    /// <summary>Gets or sets the associated meta topic.</summary>
    public CourseMetaTopic MetaTopic { get; set; } = null!;

    /// <summary>Gets or sets the question type reference term.</summary>
    public RefTerm QuestionType { get; set; } = null!;

    /// <summary>Gets or sets the collection of user answers for this question.</summary>
    public ICollection<AssessmentAnswer> AssessmentAnswers { get; set; } =
        new List<AssessmentAnswer>();
}
