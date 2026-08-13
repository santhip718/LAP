namespace LAP.Application.DTO.Course
{
    using System;

    /// <summary>
    /// Represents course content details within a course overview.
    /// </summary>
    public class CourseOverviewContentDto
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
        /// Gets or sets the display order of this content within the meta topic.
        /// </summary>
        public int SequenceOrder { get; set; }
    }
}
