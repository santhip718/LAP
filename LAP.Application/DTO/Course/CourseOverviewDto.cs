namespace LAP.Application.DTO.Course
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a comprehensive overview of a course, including its content and assessment details.
    /// </summary>
    public class CourseOverviewDto : CourseDetailDto
    {
        /// <summary>
        /// Gets or sets the number of users enrolled in this course.
        /// </summary>
        public int EnrollmentCount { get; set; }

        /// <summary>
        /// Gets or sets the title of the course assessment, if available.
        /// </summary>
        public string? AssessmentTitle { get; set; }

        /// <summary>
        /// Gets or sets the total marks achievable in the course assessment.
        /// </summary>
        public int TotalMark { get; set; }

        /// <summary>
        /// Gets or sets the minimum marks required to pass the course assessment.
        /// </summary>
        public int PassingMark { get; set; }

        /// <summary>
        /// Gets or sets the topic for this course overview (without content type and file details).
        /// </summary>
        public ICollection<CourseOverviewMetaTopicDto>? Topic { get; set; }
    }
}
