namespace LAP.Application.DTO.User
{
    /// <summary>
    /// Represents a user profile summary including enrollment and completion counts.
    /// </summary>
    public class UserProfileDto : UserDetailDto
    {
        /// <summary>
        /// Gets or sets the total number of enrollments for the user.
        /// </summary>
        public int EnrollmentCount { get; set; }

        /// <summary>
        /// Gets or sets the number of courses the user has completed.
        /// </summary>
        public int CompletedCourses { get; set; }
    }
}
