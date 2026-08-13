using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Repository for managing user course progress entities with completion tracking support.
/// </summary>
public class UserCourseProgressRepository
    : BaseRepository<UserCourseProgress>,
        IUserCourseProgressRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCourseProgressRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public UserCourseProgressRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<UserCourseProgress>> logger
    )
        : base(dbContext, logger) { }
}
