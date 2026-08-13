namespace LAP.Application.DTO.Course
{
    using System;
    using System.Collections.Generic;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents course content with user-specific completion progress.
    /// </summary>
    public class CourseContentProgressDto : CourseContentDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the current user has completed this content.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user completed this content, if applicable.
        /// </summary>
        public DateTime? CompletedOn { get; set; }
    }

    /// <summary>
    /// Represents a course topic containing contents with progress information.
    /// </summary>
    public class CourseTopicProgressDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the topic.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the topic.
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
        /// Gets or sets the collection of contents within this topic, including progress.
        /// </summary>
        public ICollection<CourseContentProgressDto> Contents { get; set; } = new List<CourseContentProgressDto>();
    }

    /// <summary>
    /// Represents the full content structure of a course with user progress.
    /// </summary>
    public class CourseContentResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the file path to the course thumbnail image.
        /// </summary>
        public string? ThumbnailImg { get; set; }

        /// <summary>
        /// Gets or sets the collection of topics and their contents with progress.
        /// </summary>
        public ICollection<CourseTopicProgressDto> Topic { get; set; } = new List<CourseTopicProgressDto>();
    }
}
