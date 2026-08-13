using System;
using System.IO;
using AutoMapper;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.CourseContent;
using LAP.Application.Interface.IService;
using LAP.Application.Options;
using LAP.Domain.Entity;
using Microsoft.Extensions.Options;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for course-related mappings.
/// </summary>
public class CourseMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CourseMappingProfile"/> class.
    /// </summary>
    public CourseMappingProfile()
    {
        CreateMap<CreateCourseRequestDto, Course>()
            .ForMember(d => d.ThumbnailImgPath, o => o.Ignore())
            .ForMember(d => d.OverallRating, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.IsDrafted, o => o.MapFrom(s => s.IsDrafted))
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.SubCategory, o => o.Ignore())
            .ForMember(d => d.DifficultyLevel, o => o.Ignore())
            .ForMember(d => d.CreatedByUser, o => o.Ignore())
            .ForMember(d => d.Topics, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.Reviews, o => o.Ignore())
            .ForMember(d => d.ForumMessages, o => o.Ignore())
            .ForMember(d => d.Assessment, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.DateCreated, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.DateUpdated, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());

        CreateMap<Course, CourseSummaryDto>()
            .ForMember(d => d.IsDrafted, o => o.MapFrom(s => s.IsDrafted))
            .ForMember(d => d.ThumbnailImg, o => o.MapFrom(s => s.ThumbnailImgPath));
        CreateMap<Course, CourseDetailDto>().IncludeBase<Course, CourseSummaryDto>();

        CreateMap<Course, CourseOverviewDto>()
            .IncludeBase<Course, CourseDetailDto>()
            .ForMember(
                d => d.AssessmentTitle,
                o => o.MapFrom(s => s.Assessment != null ? s.Assessment.Title : null)
            )
            .ForMember(
                d => d.TotalMark,
                o => o.MapFrom(s => s.Assessment != null ? s.Assessment.TotalMark : 0)
            )
            .ForMember(
                d => d.PassingMark,
                o => o.MapFrom(s => s.Assessment != null ? s.Assessment.PassingMark : 0)
            )
            .ForMember(d => d.EnrollmentCount, o => o.MapFrom(s => s.Enrollments.Count))
            .ForMember(d => d.Topic, o => o.MapFrom(s => s.Topics));

        CreateMap<CourseMetaTopic, CourseOverviewMetaTopicDto>()
            .ForMember(d => d.MetaSequenceOrder, o => o.MapFrom(s => s.SequenceOrder))
            .ForMember(d => d.MetaDurationMinute, o => o.MapFrom(s => s.DurationMinute));

        CreateMap<CourseContent, CourseOverviewContentDto>();

        CreateMap<CourseMetaTopic, CourseMetaTopicDto>();

        CreateMap<CourseMetaTopic, CourseTopicProgressDto>()
            .ForMember(d => d.MetaSequenceOrder, o => o.MapFrom(s => s.SequenceOrder))
            .ForMember(d => d.MetaDurationMinute, o => o.MapFrom(s => s.DurationMinute));

        CreateMap<Course, CourseContentResponseDto>()
            .ForMember(d => d.CourseId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ThumbnailImg, o => o.Ignore())
            .ForMember(d => d.Topic, o => o.MapFrom(s => s.Topics));

        CreateMap<CourseContent, CourseContentDto>()
            .ForMember(
                d => d.VideoUrl,
                o => o.MapFrom(s => CreateUri(s.VideoUrl))
            );

        CreateMap<CourseContent, CourseContentProgressDto>()
            .IncludeBase<CourseContent, CourseContentDto>()
            .ForMember(
                d => d.IsCompleted,
                o => o.MapFrom(s => s.UserCourseProgresses.Any(p => p.IsCompleted))
            )
            .ForMember(
                d => d.CompletedOn,
                o =>
                    o.MapFrom(s =>
                        s.UserCourseProgresses.OrderByDescending(p => p.CompletedOn)
                            .Select(p => p.CompletedOn)
                            .FirstOrDefault()
                    )
            );

        CreateMap<Enrollment, CourseProgressResponseDto>()
            .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id));

        CreateMap<CourseContent, CourseContentDetailDto>()
            .ForMember(d => d.ContentType, o => o.MapFrom(s => s.ContentType.Name))
            .ForMember(d => d.MetaTopicName, o => o.MapFrom(s => s.MetaTopic.Name))
            .ForMember(d => d.MetaSequenceOrder, o => o.MapFrom(s => s.MetaTopic.SequenceOrder))
            .ForMember(d => d.VideoUrl, o => o.MapFrom(s => s.VideoUrl))
            .ForMember(d => d.PdfBase64, o => o.Ignore())
            .ForMember(d => d.PreviousContentId, o => o.Ignore())
            .ForMember(d => d.NextContentId, o => o.Ignore())
            .ForMember(d => d.IsCompleted, o => o.Ignore())
            .ForMember(d => d.CompletedOn, o => o.Ignore());

        CreateMap<CreateCourseContentRequestDto, CourseContent>()
            .ForMember(d => d.MetaTopicId, o => o.Ignore())
            .ForMember(d => d.PdfFilePath, o => o.Ignore())
            .ForMember(d => d.MetaTopic, o => o.Ignore())
            .ForMember(d => d.ContentType, o => o.Ignore())
            .ForMember(d => d.UserCourseProgresses, o => o.Ignore())
            .ForMember(
                d => d.VideoUrl,
                o => o.MapFrom(s => s.VideoUrl != null ? s.VideoUrl.ToString() : null)
            )
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.DateCreated, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.DateUpdated, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());

        CreateMap<UpdateCourseRequestDto, Course>()
            .ForMember(d => d.Title, o => o.Condition(s => !string.IsNullOrEmpty(s.Title)))
            .ForMember(
                d => d.Description,
                o => o.Condition(s => !string.IsNullOrEmpty(s.Description))
            )
            .ForMember(d => d.CategoryId, o => o.Condition(s => s.CategoryId.HasValue))
            .ForMember(d => d.SubCategoryId, o => o.Condition(s => s.SubCategoryId.HasValue))
            .ForMember(
                d => d.DifficultyLevelId,
                o => o.Condition(s => s.DifficultyLevelId.HasValue)
            )
            .ForMember(d => d.DurationMinute, o => o.Condition(s => s.DurationMinute.HasValue))
            .ForMember(d => d.IsDrafted, o => o.Condition(s => s.IsDrafted.HasValue))
            .ForMember(d => d.ThumbnailImgPath, o => o.Ignore())
            .ForMember(d => d.OverallRating, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.SubCategory, o => o.Ignore())
            .ForMember(d => d.DifficultyLevel, o => o.Ignore())
            .ForMember(d => d.CreatedByUser, o => o.Ignore())
            .ForMember(d => d.Topics, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.Reviews, o => o.Ignore())
            .ForMember(d => d.ForumMessages, o => o.Ignore())
            .ForMember(d => d.Assessment, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.DateCreated, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.DateUpdated, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());
    }

    private static Uri? CreateUri(string? videoUrl)
    {
        if (string.IsNullOrEmpty(videoUrl))
            return null;
        if (Uri.TryCreate(videoUrl, UriKind.Absolute, out var uri))
            return uri;
        return null;
    }
}
