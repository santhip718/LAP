namespace LAP.Application.DTO.Common
{
    /// <summary>
    /// Represents an error entry for a specific row in a bulk operation.
    /// </summary>
    public class Errors
    {
        /// <summary>
        /// Gets or sets the row number where the error occurred.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the error message describing the issue.
        /// </summary>
        public string Message { get; set; }
    }
}
