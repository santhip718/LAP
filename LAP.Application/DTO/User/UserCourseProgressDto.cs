namespace LAP.Application.DTO.User
{
    using System;

    /// <summary>
    /// Represents the progress of a user on a specific course content item.
    /// </summary>
    public class UserCourseProgressDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the progress record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the enrollment.
        /// </summary>
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course content.
        /// </summary>
        public Guid CourseContentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content item is completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was completed, if applicable.
        /// </summary>
        public DateTimeOffset? CompletedOn { get; set; }
    }
}
