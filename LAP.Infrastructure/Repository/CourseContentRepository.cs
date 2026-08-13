using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Repository for managing course content entities with meta topic and navigation support.
/// </summary>
public class CourseContentRepository : BaseRepository<CourseContent>, ICourseContentRepository
{
    public CourseContentRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<CourseContent>> logger
    )
        : base(dbContext, logger) { }
}
