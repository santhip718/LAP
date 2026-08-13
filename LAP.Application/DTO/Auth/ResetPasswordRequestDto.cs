namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents a request to reset the user's password using a reset token.
    /// </summary>
    public class ResetPasswordRequestDto
    {
        /// <summary>
        /// Gets or sets the reset token received via email.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the new password the user wishes to set.
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
