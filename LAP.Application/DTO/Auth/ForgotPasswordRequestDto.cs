namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents a request to initiate the forgot password flow by providing the user's email address.
    /// </summary>
    public class ForgotPasswordRequestDto
    {
        /// <summary>
        /// Gets or sets the email address of the user requesting a password reset.
        /// </summary>
        public string Email { get; set; }
    }
}
