namespace LAP.Application.DTO.User
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a summary of a user's basic information and roles.
    /// </summary>
    public class UserSummaryDto
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
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public ICollection<string>? Roles { get; set; }
    }
}
