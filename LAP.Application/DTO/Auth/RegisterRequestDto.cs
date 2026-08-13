namespace LAP.Application.DTO.Auth
{
    using System;

    /// <summary>
    /// Represents a registration request containing the details required to create a new user account.
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// Gets or sets the password for the new account.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Gets or sets the mobile number of the user.
        /// </summary>
        public string MobileNumber { get; set; } = default!;

        /// <summary>
        /// Gets or sets the unique identifier of the user's designation.
        /// </summary>
        public Guid DesignationId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user's gender.
        /// </summary>
        public Guid GenderId { get; set; }
    }
}
