using AutoMapper;
using LAP.Application.DTO.Enrollment;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for enrollment-related mappings.
/// </summary>
public class EnrollmentMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentMappingProfile"/> class.
    /// </summary>
    public EnrollmentMappingProfile()
    {
        CreateMap<Enrollment, EnrollmentDetailDto>()
            .ForMember(d => d.ProgressPercentage, o => o.MapFrom(s => (double)s.ProgressPercentage))
            .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Title))
            .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.Course.Category))
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User.Person.FullName));
    }
}
