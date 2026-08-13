namespace LAP.Application.DTO.Assessment;

/// <summary>
/// DTO representing a question imported from an Excel file.
/// </summary>
public class QuestionImportDto
{
    /// <summary>
    /// Gets or sets the question text.
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the question type.
    /// </summary>
    public string QuestionTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the associated meta topic.
    /// </summary>
    public string MetaTopicName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first answer option.
    /// </summary>
    public string Option1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the second answer option.
    /// </summary>
    public string Option2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the third answer option.
    /// </summary>
    public string Option3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the fourth answer option.
    /// </summary>
    public string Option4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correct answer.
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the weight assigned to the question.
    /// </summary>
    public int Weight { get; set; }
}
