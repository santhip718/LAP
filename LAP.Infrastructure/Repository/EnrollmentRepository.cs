using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Repository for managing enrollment entities with progress tracking support.
/// </summary>
public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance for logging data access activity.</param>
    public EnrollmentRepository(
        LearningAssessmentDbContext context,
        ICustomLogger<BaseRepository<Enrollment>> logger
    )
        : base(context, logger) { }
}
