using System.Linq;
using AutoMapper;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Enrollment;
using LAP.Application.DTO.User;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for user-related mappings.
/// </summary>
public class UserMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMappingProfile"/> class.
    /// </summary>
    public UserMappingProfile()
    {
        CreateMap<RefTerm, RefTermDto>();

        CreateMap<User, UserDetailDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Person.FullName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Person.Email))
            .ForMember(d => d.MobileNumber, o => o.MapFrom(s => s.Person.MobileNumber))
            .ForMember(d => d.Designation, o => o.MapFrom(s => s.Person.Designation))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Person.Gender))
            .ForMember(d => d.CurrentTier, o => o.MapFrom(s => s.CurrentTier))
            .ForMember(
                d => d.Roles,
                o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name).ToList())
            )
            .ForMember(d => d.DateCreated, o => o.MapFrom(s => s.DateCreated));

        CreateMap<User, UserSummaryDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Person.FullName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Person.Email))
            .ForMember(
                d => d.Roles,
                o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name).ToList())
            );

        CreateMap<User, UserProfileDto>()
            .IncludeBase<User, UserDetailDto>()
            .ForMember(d => d.EnrollmentCount, o => o.MapFrom(s => s.Enrollments.Count))
            .ForMember(
                d => d.CompletedCourses,
                o => o.MapFrom(s => s.Enrollments.Count(e => e.CompletedOn.HasValue))
            );

        CreateMap<User, UserEnrichedDto>()
            .IncludeBase<User, UserDetailDto>()
            .ForMember(d => d.TotalEnrolledCourses, o => o.MapFrom(s => s.Enrollments.Count))
            .ForMember(
                d => d.CompletedCourses,
                o => o.MapFrom(s => s.Enrollments.Count(e => e.CompletedOn.HasValue))
            )
            .ForMember(d => d.EnrolledCourses, o => o.MapFrom(s => s.Enrollments));

        CreateMap<Enrollment, EnrolledCourseDto>()
            .ForMember(d => d.CourseId, o => o.MapFrom(s => s.CourseId))
            .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Title))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Course.Category))
            .ForMember(d => d.DifficultyLevel, o => o.MapFrom(s => s.Course.DifficultyLevel))
            .ForMember(d => d.EnrolledOn, o => o.MapFrom(s => s.EnrolledOn))
            .ForMember(d => d.CompletedOn, o => o.MapFrom(s => s.CompletedOn))
            .ForMember(
                d => d.ProgressPercentage,
                o => o.MapFrom(s => (double)s.ProgressPercentage)
            );
    }
}
