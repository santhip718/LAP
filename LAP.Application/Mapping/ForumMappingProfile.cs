using AutoMapper;
using LAP.Application.DTO.Forum;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for forum-related mappings.
/// </summary>
public class ForumMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumMappingProfile"/> class.
    /// </summary>
    public ForumMappingProfile()
    {
        CreateMap<ForumMessage, ForumMessageDto>()
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User.Person.FullName));
    }
}
