using AutoMapper;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Review;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for review-related mappings.
/// </summary>
public class ReviewMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewMappingProfile"/> class.
    /// </summary>
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User.Person.FullName));

        CreateMap<CreateReviewRequestDto, Review>()
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CourseId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.DateCreated, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.DateUpdated, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());

        CreateMap<UpdateReviewRequestDto, Review>()
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CourseId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.DateCreated, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.DateUpdated, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
