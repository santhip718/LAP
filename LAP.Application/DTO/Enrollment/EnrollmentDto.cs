namespace LAP.Application.DTO.Enrollment
{
    using System;

    /// <summary>
    /// Represents a user enrollment in a course.
    /// </summary>
    public class EnrollmentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the enrollment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user enrolled.
        /// </summary>
        public DateTimeOffset EnrolledOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the course was completed, if applicable.
        /// </summary>
        public DateTimeOffset? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage of the enrollment.
        /// </summary>
        public double ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enrollment is active.
        /// </summary>
        public bool EnrollmentStatus { get; set; }
    }
}
