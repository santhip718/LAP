using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Domain.Entity;

namespace LAP.Application.Mapping;

/// <summary>
/// AutoMapper profile for leaderboard-related mappings.
/// </summary>
public class LeaderboardMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardMappingProfile"/> class.
    /// </summary>
    public LeaderboardMappingProfile()
    {
        CreateMap<User, LeaderboardDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Person.FullName))
            .ForMember(
                dest => dest.OverallWeightedScore,
                opt => opt.MapFrom(src => src.OverallWeightedScore)
            )
            .ForMember(
                dest => dest.WeightedScore,
                opt => opt.MapFrom(src => src.OverallWeightedScore)
            )
            .ForMember(dest => dest.Rank, opt => opt.Ignore());

        CreateMap<AssessmentHistory, LeaderboardDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.FullName))
            .ForMember(
                dest => dest.OverallWeightedScore,
                opt => opt.MapFrom(src => src.WeightedScore)
            )
            .ForMember(
                dest => dest.WeightedScore,
                opt => opt.MapFrom(src => src.WeightedScore)
            )
            .ForMember(dest => dest.Rank, opt => opt.Ignore());
    }
}
