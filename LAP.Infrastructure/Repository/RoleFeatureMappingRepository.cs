using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IRoleFeatureMappingRepository"/> for role-feature mapping data access.
/// </summary>
public class RoleFeatureMappingRepository
    : BaseRepository<RoleFeatureMapping>,
        IRoleFeatureMappingRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleFeatureMappingRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public RoleFeatureMappingRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<RoleFeatureMapping>> logger
    )
        : base(dbContext, logger) { }
}
