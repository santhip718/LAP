namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents a request to create a new assessment.
    /// </summary>
    public class CreateAssessmentRequestDto
    {
        /// <summary>
        /// Gets or sets the course identifier this assessment belongs to.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the assessment title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the assessment description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the total mark for the assessment.
        /// </summary>
        public int TotalMark { get; set; }

        /// <summary>
        /// Gets or sets the passing mark for the assessment.
        /// </summary>
        public int PassingMark { get; set; }

        /// <summary>
        /// Gets or sets the duration of the assessment in minutes.
        /// </summary>
        public int DurationMinute { get; set; }

        /// <summary>
        /// Gets or sets the file containing assessment questions.
        /// </summary>
        public byte[]? QuestionFile { get; set; }
    }
}
