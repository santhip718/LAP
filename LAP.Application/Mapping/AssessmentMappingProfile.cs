using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for assessment-related mappings.
/// </summary>
public class AssessmentMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentMappingProfile"/> class.
    /// </summary>
    public AssessmentMappingProfile()
    {
        CreateMap<Assessment, AssessmentOverviewDto>();

        CreateMap<Course, AssessmentCourseDto>();

        CreateMap<AssessmentHistory, AssessmentHistoryDto>()
            .ForMember(
                dest => dest.StartedOn,
                opt => opt.MapFrom(src => new DateTimeOffset(src.StartedOn, TimeSpan.Zero))
            )
            .ForMember(
                dest => dest.CompletedOn,
                opt =>
                    opt.MapFrom(src =>
                        src.CompletedOn.HasValue
                            ? new DateTimeOffset(src.CompletedOn.Value, TimeSpan.Zero)
                            : DateTimeOffset.MinValue
                    )
            )
            .ForMember(dest => dest.TierAwarded, opt => opt.MapFrom(src => src.TierAwarded));

        CreateMap<AssessmentAnswer, AnswerFeedbackDto>()
            .ForMember(
                dest => dest.QuestionText,
                opt => opt.MapFrom(src => src.Question.QuestionText)
            )
            .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.Question.Answer));

        CreateMap<Question, QuestionDto>();

        CreateMap<RefTerm, RefTermDto>();

        CreateMap<UpdateAssessmentRequestDto, Assessment>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateQuestionRequestDto, Question>()
            .ForMember(dest => dest.OptionList, opt => opt.MapFrom(src => src.OptionList))
            .ForMember(dest => dest.MetaTopicId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<QuestionImportDto, Question>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
            .ForMember(
                dest => dest.OptionList,
                opt =>
                    opt.MapFrom(src =>
                        new List<string> { src.Option1, src.Option2, src.Option3, src.Option4 }
                            .Where(o => !string.IsNullOrWhiteSpace(o))
                            .ToList()
                    )
            )
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer.Trim()))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.AssessmentId, opt => opt.Ignore())
            .ForMember(dest => dest.MetaTopicId, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.Assessment, opt => opt.Ignore())
            .ForMember(dest => dest.MetaTopic, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionType, opt => opt.Ignore())
            .ForMember(dest => dest.AssessmentAnswers, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        CreateMap<AssessmentHistory, AssessmentHistoryDto>()
            .ForMember(d => d.TierAwarded, o => o.MapFrom(s => s.TierAwarded))
            .ForMember(d => d.Passed, o => o.MapFrom(s => s.Assessment != null && s.Score >= s.Assessment.PassingMark));

        CreateMap<AssessmentHistory, AssessmentHistoryItemDto>()
            .ForMember(d => d.AssessmentHistoryId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AssessmentTitle, o => o.MapFrom(s => s.Assessment.Title))
            .ForMember(d => d.CourseId, o => o.MapFrom(s => s.Assessment.Course.Id))
            .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Assessment.Course.Title))
            .ForMember(d => d.AttemptedOn, o => o.MapFrom(s => s.CompletedOn ?? s.StartedOn))
            .ForMember(d => d.Passed, o => o.MapFrom(s => s.Assessment != null && s.Score >= s.Assessment.PassingMark));
    }
}
