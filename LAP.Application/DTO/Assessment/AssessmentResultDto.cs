namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents the complete result of a submitted assessment.
    /// </summary>
    public class AssessmentResultDto
    {
        /// <summary>
        /// Gets or sets the assessment history identifier.
        /// </summary>
        public Guid AssessmentHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the overall score.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the weighted overall score.
        /// </summary>
        public decimal WeightedScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the assessment was passed.
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        /// Gets or sets the tier awarded based on performance.
        /// </summary>
        public RefTermDto? TierAwarded { get; set; }

        /// <summary>
        /// Gets or sets the list of weak topics identified.
        /// </summary>
        public ICollection<WeakTopicDto>? WeakTopics { get; set; }

        /// <summary>
        /// Gets or sets the collection of per-question answer feedback.
        /// </summary>
        public ICollection<AnswerFeedbackDto>? Answers { get; set; }
    }
}
