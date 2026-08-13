namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents a single assessment history record in a user's assessment history list.
    /// </summary>
    public class AssessmentHistoryItemDto
    {
        /// <summary>
        /// Gets or sets the assessment history identifier.
        /// </summary>
        public Guid AssessmentHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the assessment identifier.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the assessment title.
        /// </summary>
        public string AssessmentTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the course title.
        /// </summary>
        public string CourseTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the assessment was attempted.
        /// </summary>
        public DateTimeOffset AttemptedOn { get; set; }

        /// <summary>
        /// Gets or sets the raw score achieved (sum of question weights for correct answers).
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the weighted score percentage achieved.
        /// </summary>
        public decimal WeightedScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the assessment was passed.
        /// </summary>
        public bool Passed { get; set; }
    }
}
