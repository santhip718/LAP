namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Provides an overview of an assessment including its settings and associated course.
    /// </summary>
    public class AssessmentOverviewDto
    {
        /// <summary>
        /// Gets or sets the assessment identifier.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets the course associated with this assessment.
        /// </summary>
        public AssessmentCourseDto? Course { get; set; }
    }
}
