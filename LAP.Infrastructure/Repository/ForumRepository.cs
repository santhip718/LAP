using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Repository for managing forum message data access.
/// </summary>
public class ForumRepository : BaseRepository<ForumMessage>, IForumRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The application logger.</param>
    public ForumRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<ForumMessage>> logger
    )
        : base(dbContext, logger) { }
}
