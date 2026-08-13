namespace LAP.Application.DTO.Course
{
    using System;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents the content of a course within a meta topic, such as a video or PDF.
    /// </summary>
    public class CourseContentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course content.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent meta topic.
        /// </summary>
        public Guid MetaTopicId { get; set; }

        /// <summary>
        /// Gets or sets the title of the content.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the content type (e.g., video, PDF).
        /// </summary>
        public RefTermDto? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the URL of the video content.
        /// </summary>
        public Uri? VideoUrl { get; set; }

        /// <summary>
        /// Gets or sets the file path of the PDF content.
        /// </summary>
        public string? PdfFilePath { get; set; }

        /// <summary>
        /// Gets or sets the display order of this content within the meta topic.
        /// </summary>
        public int SequenceOrder { get; set; }
    }
}
