using AutoMapper;
using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Leaderboard.Query;

/// <summary>
/// Query to retrieve the course-specific leaderboard.
/// </summary>
/// <param name="CourseId">The course identifier.</param>
/// <param name="PageSize">The number of items to retrieve.</param>
public record GetLeaderboardByCourseIdQuery(
    Guid CourseId,
    int PageSize = CommonConstants.DEFAULT_PAGE_SIZE
) : IRequest<List<LeaderboardDto>>;

/// <summary>
/// Validates <see cref="GetLeaderboardByCourseIdQuery"/> ensuring the course exists.
/// </summary>
public class GetLeaderboardByCourseIdValidator : AbstractValidator<GetLeaderboardByCourseIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetLeaderboardByCourseIdValidator"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service for course existence check.</param>
    public GetLeaderboardByCourseIdValidator(IAssessmentService assessmentService)
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");

        RuleFor(x => x.CourseId)
            .MustAsync(async (id, token) => await assessmentService.CourseExistsAsync(id, token))
            .WithMessage("The specified course does not exist")
            .When(x => x.CourseId != Guid.Empty);
    }
}

/// <summary>
/// Handles retrieval of the course leaderboard.
/// Business logic: group by UserId, take max WeightedScore per user, break ties by lowest duration, take top 25, assign rank.
/// </summary>
public class GetLeaderboardByCourseIdHandler
    : IRequestHandler<GetLeaderboardByCourseIdQuery, List<LeaderboardDto>>
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ICustomLogger<GetLeaderboardByCourseIdHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLeaderboardByCourseIdHandler"/> class.
    /// </summary>
    /// <param name="leaderboardService">The leaderboard service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetLeaderboardByCourseIdHandler(
        ILeaderboardService leaderboardService,
        ICustomLogger<GetLeaderboardByCourseIdHandler> logger,
        IMapper mapper
    )
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve the course leaderboard.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of leaderboard entries ranked by weighted score and duration.</returns>
    public async Task<List<LeaderboardDto>> Handle(
        GetLeaderboardByCourseIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Started retrieving leaderboard for course: {CourseId}", request.CourseId);

        List<AssessmentHistory> history = await _leaderboardService.GetLeaderboardByCourseIdAsync(
            request.CourseId,
            cancellationToken
        );

        List<AssessmentHistory> topEntry = history
            .GroupBy(ah => ah.UserId)
            .Select(g =>
                g.OrderByDescending(ah => ah.WeightedScore)
                    .ThenBy(ah => ah.CompletedOn!.Value - ah.StartedOn)
                    .First()
            )
            .OrderByDescending(ah => ah.WeightedScore)
            .ThenBy(ah => ah.CompletedOn!.Value - ah.StartedOn)
            .Take(request.PageSize)
            .ToList();

        List<LeaderboardDto> leaderboard = _mapper.Map<List<LeaderboardDto>>(topEntry);

        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }

        _logger.LogInfo(
            "Completed retrieving leaderboard for course: {CourseId}",
            request.CourseId
        );

        return leaderboard;
    }
}
