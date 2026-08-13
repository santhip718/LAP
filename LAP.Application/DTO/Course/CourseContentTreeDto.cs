namespace LAP.Application.DTO.Course
{
    using System;
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents the hierarchical content tree of a course, organized by topics.
    /// </summary>
    public class CourseContentTreeDto
    {
        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the collection of topics within the course.
        /// </summary>
        public ICollection<Topics>? Topics { get; set; }
    }
}
