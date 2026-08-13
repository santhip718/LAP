namespace LAP.Application.DTO
{
    /// <summary>
    /// Represents the context of a test request including user authentication details.
    /// </summary>
    public class TestRequestContextDto
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }
    }
}
