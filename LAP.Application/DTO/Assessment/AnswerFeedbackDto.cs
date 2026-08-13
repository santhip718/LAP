namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents feedback for a single answer in an assessment result.
    /// </summary>
    public class AnswerFeedbackDto
    {
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the answer selected by the user.
        /// </summary>
        public string? SelectedAnswer { get; set; }

        /// <summary>
        /// Gets or sets the correct answer for the question.
        /// </summary>
        public string? CorrectAnswer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the selected answer is correct.
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the score obtained for this question.
        /// </summary>
        public double ObtainedScore { get; set; }
    }
}
