namespace LAP.Application.DTO.Course
{
    using System;
    using LAP.Application.DTO.Common;
    using LAP.Application.DTO.User;

    /// <summary>
    /// Provides detailed information about a course, extending the summary with description and metadata.
    /// </summary>
    public class CourseDetailDto : CourseSummaryDto
    {
        /// <summary>
        /// Gets or sets the detailed description of the course.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the sub-category of the course.
        /// </summary>
        public RefTermDto? SubCategory { get; set; }

        /// <summary>
        /// Gets or sets the user who created the course.
        /// </summary>
        public UserSummaryDto? CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the course was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the course was last updated.
        /// </summary>
        public DateTimeOffset DateUpdated { get; set; }
    }
}
