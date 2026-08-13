namespace LAP.Application.DTO.CourseContent
{
    using System;

    /// <summary>
    /// Represents the response returned after updating the completion status of course content.
    /// </summary>
    public class UpdateContentCompletionStatusResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course content.
        /// </summary>
        public Guid CourseContentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course content is completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was completed.
        /// </summary>
        public DateTime? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the overall course progress percentage.
        /// </summary>
        public decimal CourseProgressPercentage { get; set; }
    }
}
