using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LAP.Application.DTO.Course;

/// <summary>
/// Represents a request to create a new course.
/// </summary>
public class CreateCourseRequestDto
{
    /// <summary>
    /// Gets or sets the title of the course.
    /// </summary>
    [FromForm(Name = "title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the course.
    /// </summary>
    [FromForm(Name = "description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the course category.
    /// </summary>
    [FromForm(Name = "category_id")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the course sub-category.
    /// </summary>
    [FromForm(Name = "sub_category_id")]
    public Guid SubCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the difficulty level.
    /// </summary>
    [FromForm(Name = "difficulty_level_id")]
    public Guid DifficultyLevelId { get; set; }

    /// <summary>
    /// Gets or sets the total duration of the course in minutes.
    /// </summary>
    [FromForm(Name = "duration_minute")]
    public int DurationMinute { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image file for the course.
    /// </summary>
    [FromForm(Name = "thumbnail_img")]
    public IFormFile? ThumbnailImg { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the course is drafted.
    /// </summary>
    [FromForm(Name = "is_drafted")]
    public bool IsDrafted { get; set; }
}
