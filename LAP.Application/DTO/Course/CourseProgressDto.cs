namespace LAP.Application.DTO.Course
{
    using System;

    /// <summary>
    /// Represents the progress of a student's enrollment in a course.
    /// </summary>
    public class CourseProgressDto
    {
        /// <summary>
        /// Gets or sets the enrollment identifier.
        /// </summary>
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage completed (0 to 100).
        /// </summary>
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the course was completed, if applicable.
        /// </summary>
        public DateTimeOffset? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enrollment is active.
        /// </summary>
        public bool EnrollmentStatus { get; set; }
    }
}
