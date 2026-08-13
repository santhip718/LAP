namespace LAP.Application.DTO.Common
{
    using System;
    using LAP.Application.DTO.Course;

    /// <summary>
    /// Represents course content with completion tracking information.
    /// </summary>
    public class Contents : CourseContentDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the content has been completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was completed.
        /// </summary>
        public DateTimeOffset CompletedOn { get; set; }
    }
}
