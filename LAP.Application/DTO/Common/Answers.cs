namespace LAP.Application.DTO.Common
{
    using System;

    /// <summary>
    /// Represents a user's answer to a specific question.
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// Gets or sets the unique identifier of the question.
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the answer selected by the user.
        /// </summary>
        public string SelectedAnswer { get; set; }
    }
}
