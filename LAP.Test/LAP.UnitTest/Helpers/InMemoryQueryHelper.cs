using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LAP.UnitTest.Helpers;

internal static class InMemoryQueryHelper
{
    internal static LearningAssessmentDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<LearningAssessmentDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString("N"))
            .ConfigureWarnings(w => w.Ignore(
                Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var requestContextMock = new Mock<IRequestContext>();
        return new LearningAssessmentDbContext(options, requestContextMock.Object);
    }

    internal static Mock<ICustomLogger<T>> CreateLoggerMock<T>()
        where T : class
        => new();

    internal static CourseRepository CreateCourseRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<Course>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<Course>>();
        return new CourseRepository(context, loggerMock.Object);
    }

    internal static ReviewRepository CreateReviewRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<Review>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<Review>>();
        return new ReviewRepository(context, loggerMock.Object);
    }

    internal static CourseContentRepository CreateCourseContentRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<CourseContent>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<CourseContent>>();
        return new CourseContentRepository(context, loggerMock.Object);
    }

    internal static EnrollmentRepository CreateEnrollmentRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<Enrollment>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<Enrollment>>();
        return new EnrollmentRepository(context, loggerMock.Object);
    }

    internal static UserCourseProgressRepository CreateUserCourseProgressRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<UserCourseProgress>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<UserCourseProgress>>();
        return new UserCourseProgressRepository(context, loggerMock.Object);
    }

    internal static AssessmentHistoryRepository CreateAssessmentHistoryRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<AssessmentHistory>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<AssessmentHistory>>();
        return new AssessmentHistoryRepository(context, loggerMock.Object);
    }

    internal static AssessmentRepository CreateAssessmentRepo(
        LearningAssessmentDbContext context,
        Mock<ICustomLogger<BaseRepository<Assessment>>>? loggerMock = null)
    {
        loggerMock ??= CreateLoggerMock<BaseRepository<Assessment>>();
        return new AssessmentRepository(context, loggerMock.Object);
    }
}
