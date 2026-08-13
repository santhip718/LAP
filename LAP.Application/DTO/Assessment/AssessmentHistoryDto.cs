namespace LAP.Application.DTO.Assessment
{
    using System;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents the history record of a completed assessment attempt.
    /// </summary>
    public class AssessmentHistoryDto
    {
        /// <summary>
        /// Gets or sets the history record identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the assessment identifier.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the assessment was started.
        /// </summary>
        public DateTimeOffset StartedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the assessment was completed.
        /// </summary>
        public DateTimeOffset CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the raw score achieved.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the weighted score achieved.
        /// </summary>
        public double WeightedScore { get; set; }

        /// <summary>
        /// Gets or sets the tier awarded for this attempt.
        /// </summary>
        public RefTermDto? TierAwarded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the assessment was passed.
        /// </summary>
        public bool Passed { get; set; }
    }
}
