using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IAssessmentAnswerRepository"/> for managing assessment answer data.
/// </summary>
public class AssessmentAnswerRepository
    : BaseRepository<AssessmentAnswer>,
        IAssessmentAnswerRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentAnswerRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public AssessmentAnswerRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<AssessmentAnswer>> logger
    )
        : base(dbContext, logger) { }
}
