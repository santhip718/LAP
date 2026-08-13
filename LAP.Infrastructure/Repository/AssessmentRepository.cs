using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Extends the base repository with assessment-specific data access methods.
/// </summary>
public class AssessmentRepository
    : BaseRepository<Assessment>,
        IAssessmentRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssessmentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public AssessmentRepository(
        LearningAssessmentDbContext context,
        ICustomLogger<BaseRepository<Assessment>> logger
    )
        : base(context, logger) { }
}
