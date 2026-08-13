namespace LAP.Application.DTO.Enrollment
{
    /// <summary>
    /// Represents a request to update the completion status of an enrollment.
    /// </summary>
    public class CompletionStatusRequestDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the course content is completed.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
