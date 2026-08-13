namespace LAP.Application.DTO.CourseContent
{
    using System;

    /// <summary>
    /// Provides detailed information about course content, including user-specific progress.
    /// </summary>
    public class CourseContentDetailDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course content.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the content.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the content type (e.g., video, PDF).
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the URL of the video content.
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded string of the PDF content.
        /// </summary>
        public string? PdfBase64 { get; set; }

        /// <summary>
        /// Gets or sets the display order of this content within the meta topic.
        /// </summary>
        public int SequenceOrder { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent meta topic.
        /// </summary>
        public Guid MetaTopicId { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent meta topic.
        /// </summary>
        public string? MetaTopicName { get; set; }

        /// <summary>
        /// Gets or sets the sequence order of the parent meta topic within the course.
        /// </summary>
        public int MetaSequenceOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content has been completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was completed.
        /// </summary>
        public DateTime? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the previous content item.
        /// </summary>
        public Guid? PreviousContentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the next content item.
        /// </summary>
        public Guid? NextContentId { get; set; }
    }
}
