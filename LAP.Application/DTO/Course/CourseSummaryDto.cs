namespace LAP.Application.DTO.Course
{
    using System;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Provides a summary of a course with basic information including category, difficulty, and rating.
    /// </summary>
    public class CourseSummaryDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the category of the course.
        /// </summary>
        public RefTermDto? Category { get; set; }

        /// <summary>
        /// Gets or sets the difficulty level of the course.
        /// </summary>
        public RefTermDto? DifficultyLevel { get; set; }

        /// <summary>
        /// Gets or sets the total duration of the course in minutes.
        /// </summary>
        public int DurationMinute { get; set; }

        /// <summary>
        /// Gets or sets the overall rating of the course.
        /// </summary>
        public decimal OverallRating { get; set; }

        /// <summary>
        /// Gets or sets the file path to the course thumbnail image.
        /// </summary>
        public string? ThumbnailImg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is drafted.
        /// </summary>
        public bool IsDrafted { get; set; }
    }
}
