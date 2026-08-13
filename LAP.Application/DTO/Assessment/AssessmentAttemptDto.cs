namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents a single assessment attempt result.
    /// </summary>
    public class AssessmentAttemptDto
    {
        /// <summary>
        /// Gets or sets the attempt number (1-based).
        /// </summary>
        public int AttemptNumber { get; set; }

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
