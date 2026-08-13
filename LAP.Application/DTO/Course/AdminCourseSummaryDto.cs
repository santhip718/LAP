namespace LAP.Application.DTO.Course
{
    /// <summary>
    /// Represents a summary of courses from an admin perspective, including counts and enrollment statistics.
    /// </summary>
    public class AdminCourseSummaryDto
    {
        /// <summary>
        /// Gets or sets the total number of courses.
        /// </summary>
        public int TotalCourses { get; set; }

        /// <summary>
        /// Gets or sets the number of published courses.
        /// </summary>
        public int PublishedCourses { get; set; }

        /// <summary>
        /// Gets or sets the number of draft courses.
        /// </summary>
        public int DraftCourses { get; set; }

        /// <summary>
        /// Gets or sets the total number of enrollments across all courses.
        /// </summary>
        public int TotalEnrollments { get; set; }

        /// <summary>
        /// Gets or sets the number of active students currently enrolled.
        /// </summary>
        public int ActiveStudents { get; set; }
    }
}
