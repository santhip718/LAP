using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="ITierRepository"/> for retrieving tier reference data.
/// </summary>
public class TierRepository : BaseRepository<RefTerm>, ITierRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TierRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public TierRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<RefTerm>> logger
    )
        : base(dbContext, logger) { }
}
