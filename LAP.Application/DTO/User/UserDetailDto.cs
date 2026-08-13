namespace LAP.Application.DTO.User
{
    using System;
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Provides detailed information about a user.
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the mobile number of the user.
        /// </summary>
        public string? MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the designation of the user.
        /// </summary>
        public RefTermDto? Designation { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        public RefTermDto? Gender { get; set; }

        /// <summary>
        /// Gets or sets the current tier of the user.
        /// </summary>
        public RefTermDto? CurrentTier { get; set; }

        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public ICollection<string>? Roles { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user account was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the profile image represented as a base64 Data URL.
        /// </summary>
        public string? ProfileImage { get; set; }
    }
}
