namespace LAP.Application.DTO.Enrollment
{
    using System;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents a course that a user is enrolled in, including progress and metadata.
    /// </summary>
    public class EnrolledCourseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        public string? CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the category of the course.
        /// </summary>
        public RefTermDto? Category { get; set; }

        /// <summary>
        /// Gets or sets the difficulty level of the course.
        /// </summary>
        public RefTermDto? DifficultyLevel { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user enrolled in the course.
        /// </summary>
        public DateTimeOffset EnrolledOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user completed the course, if applicable.
        /// </summary>
        public DateTimeOffset? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage of the course completion.
        /// </summary>
        public double ProgressPercentage { get; set; }
    }
}
