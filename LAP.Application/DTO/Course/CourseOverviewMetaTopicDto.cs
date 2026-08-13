namespace LAP.Application.DTO.Course
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a meta topic within a course overview.
    /// </summary>
    public class CourseOverviewMetaTopicDto
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
        /// Gets or sets the sequence order of the meta topic within the course.
        /// </summary>
        public int MetaSequenceOrder { get; set; }

        /// <summary>
        /// Gets or sets the duration of the meta topic in minutes.
        /// </summary>
        public int MetaDurationMinute { get; set; }

        /// <summary>
        /// Gets or sets the collection of content items belonging to this meta topic.
        /// </summary>
        public ICollection<CourseOverviewContentDto>? Contents { get; set; }
    }
}
