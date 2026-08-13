using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IRefreshTokenRepository"/> for managing refresh token data.
/// </summary>
public class RefreshTokenRepository
    : BaseRepository<RefreshToken>,
        IRefreshTokenRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public RefreshTokenRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<RefreshToken>> logger
    )
        : base(dbContext, logger) { }
}
