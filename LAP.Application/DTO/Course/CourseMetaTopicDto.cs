namespace LAP.Application.DTO.Course
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a meta topic within a course, containing a collection of course contents.
    /// </summary>
    public class CourseMetaTopicDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the meta topic.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the meta topic.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the display order of the meta topic within the course.
        /// </summary>
        public int SequenceOrder { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes of the meta topic.
        /// </summary>
        public int DurationMinute { get; set; }

        /// <summary>
        /// Gets or sets the collection of content items belonging to this meta topic.
        /// </summary>
        public ICollection<CourseContentDto>? Contents { get; set; }
    }
}
