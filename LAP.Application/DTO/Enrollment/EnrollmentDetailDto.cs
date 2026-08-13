namespace LAP.Application.DTO.Enrollment
{
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Provides detailed information about an enrollment, including course and user details.
    /// </summary>
    public class EnrollmentDetailDto : EnrollmentDto
    {
        /// <summary>
        /// Gets or sets the title of the enrolled course.
        /// </summary>
        public string? CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the category of the enrolled course.
        /// </summary>
        public RefTermDto? CourseCategory { get; set; }

        /// <summary>
        /// Gets or sets the full name of the enrolled user.
        /// </summary>
        public string? UserFullName { get; set; }
    }
}
