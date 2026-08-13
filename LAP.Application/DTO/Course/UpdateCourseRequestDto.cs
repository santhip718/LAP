using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LAP.Application.DTO.Course;

/// <summary>
/// Represents a request to update an existing course's details.
/// </summary>
public class UpdateCourseRequestDto
{
    [FromForm(Name = "title")]
    public string? Title { get; set; }

    [FromForm(Name = "description")]
    public string? Description { get; set; }

    [FromForm(Name = "category_id")]
    public Guid? CategoryId { get; set; }

    [FromForm(Name = "sub_category_id")]
    public Guid? SubCategoryId { get; set; }

    [FromForm(Name = "difficulty_level_id")]
    public Guid? DifficultyLevelId { get; set; }

    [FromForm(Name = "duration_minute")]
    public int? DurationMinute { get; set; }

    [FromForm(Name = "thumbnail_img")]
    public IFormFile? ThumbnailImg { get; set; }

    [FromForm(Name = "is_drafted")]
    public bool? IsDrafted { get; set; }
}
