using AutoMapper;
using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Leaderboard.Query;

/// <summary>
/// Query to retrieve the overall platform leaderboard.
/// </summary>
/// <param name="PageSize">The number of items to retrieve.</param>
public record GetOverallPlatformLeaderboardQuery(int PageSize = CommonConstants.DEFAULT_PAGE_SIZE)
    : IRequest<List<LeaderboardDto>>;

/// <summary>
/// Handles retrieval of the overall platform leaderboard.
/// Business logic: sort by OverallWeightedScore descending, take top 25, assign rank.
/// </summary>
public class GetOverallPlatformLeaderboardHandler
    : IRequestHandler<GetOverallPlatformLeaderboardQuery, List<LeaderboardDto>>
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ICustomLogger<GetOverallPlatformLeaderboardHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOverallPlatformLeaderboardHandler"/> class.
    /// </summary>
    /// <param name="leaderboardService">The leaderboard service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetOverallPlatformLeaderboardHandler(
        ILeaderboardService leaderboardService,
        ICustomLogger<GetOverallPlatformLeaderboardHandler> logger,
        IMapper mapper
    )
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve the overall platform leaderboard.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of leaderboard entries ranked by overall weighted score.</returns>
    public async Task<List<LeaderboardDto>> Handle(
        GetOverallPlatformLeaderboardQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Started retrieving overall platform leaderboard");

        List<Domain.Entity.User> user = await _leaderboardService.GetOverallLeaderboardAsync(
            cancellationToken
        );

        List<Domain.Entity.User> topUser = user.OrderByDescending(u => u.OverallWeightedScore)
            .Take(request.PageSize)
            .ToList();

        List<LeaderboardDto> leaderboard = _mapper.Map<List<LeaderboardDto>>(topUser);

        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }

        _logger.LogInfo("Completed retrieving overall platform leaderboard", leaderboard.Count);

        return leaderboard;
    }
}
