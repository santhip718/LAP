using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LAP.Application.DTO.Course;

public class UpdateCourseContentRequestDto
{
    [FromForm(Name = "course_id")]
    public Guid CourseId { get; set; }

    [FromForm(Name = "meta_topic")]
    public string MetaTopic { get; set; } = string.Empty;

    [FromForm(Name = "meta_topic_order")]
    public int? MetaTopicOrder { get; set; }

    [FromForm(Name = "meta_duration_minute")]
    public int MetaDurationMinute { get; set; }

    [FromForm(Name = "title")]
    public string Title { get; set; } = string.Empty;

    [FromForm(Name = "content_type_id")]
    public Guid ContentTypeId { get; set; }

    [FromForm(Name = "video_url")]
    public Uri? VideoUrl { get; set; }

    [FromForm(Name = "pdf_file")]
    public IFormFile? PdfFile { get; set; }

    [FromForm(Name = "sequence_order")]
    public int? SequenceOrder { get; set; }
}
