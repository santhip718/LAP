namespace LAP.Application.DTO.User
{
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;
    using LAP.Application.DTO.Enrollment;

    /// <summary>
    /// Provides enriched user information including enrollment statistics and enrolled courses.
    /// </summary>
    public class UserEnrichedDto : UserDetailDto
    {
        /// <summary>
        /// Gets or sets the total number of courses the user is enrolled in.
        /// </summary>
        public int TotalEnrolledCourses { get; set; }

        /// <summary>
        /// Gets or sets the number of courses the user has completed.
        /// </summary>
        public int CompletedCourses { get; set; }

        /// <summary>
        /// Gets or sets the collection of courses the user is enrolled in.
        /// </summary>
        public ICollection<EnrolledCourseDto>? EnrolledCourses { get; set; }
    }
}
