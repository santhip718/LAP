namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents a question within an assessment.
    /// </summary>
    public class QuestionDto
    {
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the assessment identifier this question belongs to.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the meta-topic identifier associated with this question.
        /// </summary>
        public Guid MetaTopicId { get; set; }

        /// <summary>
        /// Gets or sets the type of the question.
        /// </summary>
        public RefTermDto? QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the list of available answer options.
        /// </summary>
        public ICollection<string>? OptionList { get; set; }

        /// <summary>
        /// Gets or sets the correct answer for this question. Only populated for authorized users.
        /// </summary>
        public string? Answer { get; set; }

        /// <summary>
        /// Gets or sets the weight (mark) of this question.
        /// </summary>
        public int Weight { get; set; }
    }
}
