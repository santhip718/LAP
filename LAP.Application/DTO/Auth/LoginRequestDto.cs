namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents a login request containing the user's credentials.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}
