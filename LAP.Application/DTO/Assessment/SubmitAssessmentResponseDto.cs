namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the response returned after successfully submitting an assessment.
    /// </summary>
    public class SubmitAssessmentResponseDto
    {
        /// <summary>
        /// Gets or sets the assessment history identifier for the completed attempt.
        /// </summary>
        public Guid AssessmentHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the assessment identifier.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the course identifier associated with the assessment.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the submission status.
        /// </summary>
        public string Status { get; set; } = "Completed";

        /// <summary>
        /// Gets or sets the date and time when the assessment was started.
        /// </summary>
        public DateTime StartedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the assessment was completed.
        /// </summary>
        public DateTime CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the duration taken to complete the assessment in minutes.
        /// </summary>
        public int DurationTakenMinutes { get; set; }

        /// <summary>
        /// Gets or sets the total number of questions in the assessment.
        /// </summary>
        public int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the number of correctly answered questions.
        /// </summary>
        public int CorrectAnswer { get; set; }

        /// <summary>
        /// Gets or sets the raw score achieved (sum of question weights for correct answers).
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the weighted score percentage achieved.
        /// </summary>
        public decimal WeightedScore { get; set; }

        /// <summary>
        /// Gets or sets the course mastery score calculated from best assessment attempts.
        /// </summary>
        public decimal CourseMasteryScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the assessment was passed.
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        /// Gets or sets the name of the tier awarded based on the course mastery score.
        /// </summary>
        public string TierAwarded { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of weak topics identified from the assessment.
        /// </summary>
        public ICollection<WeakTopicDto> WeakTopic { get; set; } = new List<WeakTopicDto>();

        /// <summary>
        /// Gets or sets the collection of per-question answer review.
        /// </summary>
        public ICollection<SubmitAnswerReviewDto> Answers { get; set; } = new List<SubmitAnswerReviewDto>();
    }
}
