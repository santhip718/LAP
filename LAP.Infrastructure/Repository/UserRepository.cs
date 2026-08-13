using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IUserRepository"/> for managing user data.
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public UserRepository(
        LearningAssessmentDbContext context,
        ICustomLogger<BaseRepository<User>> logger
    )
        : base(context, logger) { }
}
