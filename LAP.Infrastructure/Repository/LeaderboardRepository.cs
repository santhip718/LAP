using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implements <see cref="ILeaderboardRepository"/> with direct DbContext access for leaderboard queries.
/// </summary>
public class LeaderboardRepository
    : BaseRepository<AssessmentHistory>,
        ILeaderboardRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public LeaderboardRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<AssessmentHistory>> logger
    )
        : base(dbContext, logger) { }
}
