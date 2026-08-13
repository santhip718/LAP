using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IAssessmentHistoryRepository"/> for managing assessment history data.
/// </summary>
public class AssessmentHistoryRepository
    : BaseRepository<AssessmentHistory>,
        IAssessmentHistoryRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentHistoryRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public AssessmentHistoryRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<AssessmentHistory>> logger
    )
        : base(dbContext, logger) { }
}
