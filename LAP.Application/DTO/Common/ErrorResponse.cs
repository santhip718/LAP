namespace LAP.Application.DTO.Common
{
    /// <summary>
    /// Represents an error response returned by the API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the detailed error description.
        /// </summary>
        public string Error { get; set; }
    }
}
