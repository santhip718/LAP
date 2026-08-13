namespace LAP.Application.DTO.Common
{
    using System;

    /// <summary>
    /// Represents a success response returned by the API.
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created or updated resource.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the success message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
