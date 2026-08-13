using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LAP.Application.DTO.Course;

/// <summary>
/// Represents a request to create course content within a meta topic.
/// </summary>
public class CreateCourseContentRequestDto
{
    /// <summary>
    /// Gets or sets the identifier of the parent course.
    /// </summary>
    [FromForm(Name = "course_id")]
    public Guid CourseId { get; set; }

    /// <summary>
    /// Gets or sets the name of the meta topic for the content.
    /// </summary>
    [FromForm(Name = "meta_topic")]
    public string MetaTopic { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display order of the meta topic.
    /// </summary>
    [FromForm(Name = "meta_topic_order")]
    public int? MetaTopicOrder { get; set; }

    /// <summary>
    /// Gets or sets the duration in minutes of the meta topic.
    /// </summary>
    [FromForm(Name = "meta_duration_minute")]
    public int MetaDurationMinute { get; set; }

    /// <summary>
    /// Gets or sets the title of the content.
    /// </summary>
    [FromForm(Name = "title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the content type.
    /// </summary>
    [FromForm(Name = "content_type_id")]
    public Guid ContentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the video content.
    /// </summary>
    [FromForm(Name = "video_url")]
    public Uri? VideoUrl { get; set; }

    /// <summary>
    /// Gets or sets the uploaded PDF file for the content.
    /// </summary>
    [FromForm(Name = "pdf_file")]
    public IFormFile? PdfFile { get; set; }

    /// <summary>
    /// Gets or sets the display order of the content within the meta topic.
    /// </summary>
    [FromForm(Name = "sequence_order")]
    public int? SequenceOrder { get; set; }
}
