using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IReviewRepository"/> for managing <see cref="Review"/> entities.
/// </summary>
public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public ReviewRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<Review>> logger
    )
        : base(dbContext, logger) { }
}
