namespace LAP.Application.DTO.Common
{
    using System.Collections.Generic;
    using LAP.Application.DTO.Course;

    /// <summary>
    /// Represents a course topic with its associated contents.
    /// </summary>
    public class Topics : CourseMetaTopicDto
    {
        /// <summary>
        /// Gets or sets the collection of contents belonging to this topic.
        /// </summary>
        public new ICollection<Contents> Contents { get; set; }
    }
}
