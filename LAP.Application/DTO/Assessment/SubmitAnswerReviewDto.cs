namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents a review of a single answer submitted as part of an assessment.
    /// </summary>
    public class SubmitAnswerReviewDto
    {
        /// <summary>Gets or sets the unique identifier of the question.</summary>
        public Guid QuestionId { get; set; }

        /// <summary>Gets or sets the text of the question.</summary>
        public string? QuestionText { get; set; }

        /// <summary>Gets or sets the answer selected by the user.</summary>
        public string? SelectedAnswer { get; set; }

        /// <summary>Gets or sets a value indicating whether the selected answer is correct.</summary>
        public bool IsCorrect { get; set; }

        /// <summary>Gets or sets the score obtained for this question.</summary>
        public int ObtainedScore { get; set; }
    }
}
