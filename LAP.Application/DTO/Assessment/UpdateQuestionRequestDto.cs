namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a request to update an existing question.
    /// </summary>
    public class UpdateQuestionRequestDto
    {
        /// <summary>
        /// Gets or sets the updated question text.
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the updated list of answer options.
        /// </summary>
        public ICollection<string>? OptionList { get; set; }

        /// <summary>
        /// Gets or sets the correct answer.
        /// </summary>
        public string? Answer { get; set; }

        /// <summary>
        /// Gets or sets the meta-topic identifier.
        /// </summary>
        public string? MetaTopicId { get; set; }

        /// <summary>
        /// Gets or sets the updated weight of the question.
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// Gets or sets the question type identifier.
        /// </summary>
        public Guid? QuestionTypeId { get; set; }
    }
}
