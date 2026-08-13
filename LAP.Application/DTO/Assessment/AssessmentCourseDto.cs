namespace LAP.Application.DTO.Assessment;

using System;
using LAP.Application.DTO.Common;

/// <summary>
/// Minimal course details for assessment overview.
/// </summary>
public class AssessmentCourseDto
{
    /// <summary>
    /// Gets or sets the course identifier.
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
    /// Gets or sets a value indicating whether the course is drafted.
    /// </summary>
    public bool IsDrafted { get; set; }
}
