namespace LAP.Application.DTO.User
{
    using System;

    /// <summary>
    /// Represents a request to update user profile details.
    /// </summary>
    public class UpdateUserRequestDto
    {
        /// <summary>
        /// Gets or sets the updated full name of the user.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated mobile number of the user.
        /// </summary>
        public string MobileNumber { get; set; } = string.Empty;

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
