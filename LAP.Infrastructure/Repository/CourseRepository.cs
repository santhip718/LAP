using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="ICourseRepository"/> for managing course data.
/// </summary>
public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CourseRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    public CourseRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<Course>> logger
    )
        : base(dbContext, logger) { }
}
