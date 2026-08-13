namespace LAP.Application.DTO.CourseContent
{
    /// <summary>
    /// Represents a request to update the completion status of course content.
    /// </summary>
    public class UpdateContentCompletionStatusRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the course content is completed.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
