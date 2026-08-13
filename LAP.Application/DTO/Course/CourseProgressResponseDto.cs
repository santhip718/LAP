namespace LAP.Application.DTO.Course
{
    using System;

    /// <summary>
    /// Represents the overall progress of a user in a specific course.
    /// </summary>
    public class CourseProgressResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the enrollment.
        /// </summary>
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the course completed by the user.
        /// </summary>
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user completed the course, if applicable.
        /// </summary>
        public DateTime? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the status of the enrollment.
        /// </summary>
        public bool EnrollmentStatus { get; set; }
    }
}
