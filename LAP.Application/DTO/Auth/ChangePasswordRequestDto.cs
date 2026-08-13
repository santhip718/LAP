namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents a request to change the authenticated user's password.
    /// </summary>
    public class ChangePasswordRequestDto
    {
        /// <summary>
        /// Gets or sets the user's current password for verification.
        /// </summary>
        public string CurrentPassword { get; set; }

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
