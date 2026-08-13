using System.Linq.Expressions;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Infrastructure.Repository;
using LAP.UnitTest.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LAP.UnitTest.Service;

public class CourseContentServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICustomLogger<CourseContentService>> _loggerMock;
    private readonly CourseContentService _courseContentService;

    public CourseContentServiceTest()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<CourseContentService>>();
        _courseContentService = new CourseContentService(
            _repositoryWrapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetContentWithMetaTopicAsync_ShouldReturnContent()
    {
        var contentId = Guid.NewGuid();
        var metaTopic = new CourseMetaTopic { Id = Guid.NewGuid(), CourseId = Guid.NewGuid() };
        var contentType = new RefTerm { Id = Guid.NewGuid(), Name = "Video" };
        var context = InMemoryQueryHelper.CreateContext();
        context.CourseContent.Add(
            new CourseContent
            {
                Id = contentId,
                MetaTopicId = metaTopic.Id,
                ContentTypeId = contentType.Id,
                IsActive = true,
                MetaTopic = metaTopic,
                ContentType = contentType,
            }
        );
        context.SaveChanges();

        var contentRepo = InMemoryQueryHelper.CreateCourseContentRepo(context);
        _repositoryWrapperMock.Setup(x => x.CourseContent).Returns(contentRepo);

        var result = await _courseContentService.GetContentWithMetaTopicAsync(contentId, default);

        Assert.Equal(contentId, result!.Id);
    }

    [Fact]
    public async Task GetPreviousContentAsync_ShouldReturnContent()
    {
        var courseId = Guid.NewGuid();
        var metaTopic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            SequenceOrder = 1,
        };
        var context = InMemoryQueryHelper.CreateContext();
        context.CourseContent.Add(
            new CourseContent
            {
                Id = Guid.NewGuid(),
                MetaTopicId = metaTopic.Id,
                IsActive = true,
                SequenceOrder = 5,
                MetaTopic = metaTopic,
            }
        );
        context.SaveChanges();

        var contentRepo = InMemoryQueryHelper.CreateCourseContentRepo(context);
        _repositoryWrapperMock.Setup(x => x.CourseContent).Returns(contentRepo);

        var result = await _courseContentService.GetPreviousContentAsync(courseId, 2, 1, default);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetNextContentAsync_ShouldReturnContent()
    {
        var courseId = Guid.NewGuid();
        var metaTopic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            SequenceOrder = 2,
        };
        var context = InMemoryQueryHelper.CreateContext();
        context.CourseContent.Add(
            new CourseContent
            {
                Id = Guid.NewGuid(),
                MetaTopicId = metaTopic.Id,
                IsActive = true,
                SequenceOrder = 1,
                MetaTopic = metaTopic,
            }
        );
        context.SaveChanges();

        var contentRepo = InMemoryQueryHelper.CreateCourseContentRepo(context);
        _repositoryWrapperMock.Setup(x => x.CourseContent).Returns(contentRepo);

        var result = await _courseContentService.GetNextContentAsync(courseId, 1, 5, default);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetEnrollmentByUserAndCourseAsync_ShouldReturnEnrollment()
    {
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Enrollment.Add(
            new Enrollment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CourseId = courseId,
                IsActive = true,
            }
        );
        context.SaveChanges();

        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Enrollment).Returns(enrollmentRepo);

        var result = await _courseContentService.GetEnrollmentByUserAndCourseAsync(
            userId,
            courseId,
            default
        );

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProgressAsync_ShouldReturnProgress()
    {
        var enrollmentId = Guid.NewGuid();
        var contentId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.UserCourseProgress.Add(
            new UserCourseProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollmentId,
                CourseContentId = contentId,
                IsActive = true,
            }
        );
        context.SaveChanges();

        var progressRepo = InMemoryQueryHelper.CreateUserCourseProgressRepo(context);
        _repositoryWrapperMock.Setup(x => x.UserCourseProgress).Returns(progressRepo);

        var result = await _courseContentService.GetProgressAsync(enrollmentId, contentId, default);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetTotalContentCountAsync_ShouldReturnCount()
    {
        var courseId = Guid.NewGuid();
        var metaTopic = new CourseMetaTopic { Id = Guid.NewGuid(), CourseId = courseId };
        var context = InMemoryQueryHelper.CreateContext();
        context.CourseContent.AddRange(
            new CourseContent
            {
                Id = Guid.NewGuid(),
                MetaTopicId = metaTopic.Id,
                IsActive = true,
                MetaTopic = metaTopic,
            },
            new CourseContent
            {
                Id = Guid.NewGuid(),
                MetaTopicId = metaTopic.Id,
                IsActive = true,
                MetaTopic = metaTopic,
            }
        );
        context.SaveChanges();

        var contentRepo = InMemoryQueryHelper.CreateCourseContentRepo(context);
        _repositoryWrapperMock.Setup(x => x.CourseContent).Returns(contentRepo);

        var result = await _courseContentService.GetTotalContentCountAsync(courseId, default);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetCompletedContentCountAsync_ShouldReturnCount()
    {
        var enrollmentId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.UserCourseProgress.AddRange(
            new UserCourseProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollmentId,
                IsCompleted = true,
                IsActive = true,
            },
            new UserCourseProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollmentId,
                IsCompleted = false,
                IsActive = true,
            },
            new UserCourseProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollmentId,
                IsCompleted = true,
                IsActive = true,
            }
        );
        context.SaveChanges();

        var progressRepo = InMemoryQueryHelper.CreateUserCourseProgressRepo(context);
        _repositoryWrapperMock.Setup(x => x.UserCourseProgress).Returns(progressRepo);

        var result = await _courseContentService.GetCompletedContentCountAsync(
            enrollmentId,
            default
        );

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task AddProgressAsync_ShouldCallRepository()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var progressRepo = InMemoryQueryHelper.CreateUserCourseProgressRepo(context);
        _repositoryWrapperMock.Setup(x => x.UserCourseProgress).Returns(progressRepo);

        var progress = new UserCourseProgress { Id = Guid.NewGuid() };
        await _courseContentService.AddProgressAsync(progress, default);
        await context.SaveChangesAsync();

        Assert.Contains(progress, context.UserCourseProgress.ToList());
    }

    [Fact]
    public async Task UpdateProgressAsync_ShouldUpdate()
    {
        var progressId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.UserCourseProgress.Add(
            new UserCourseProgress
            {
                Id = progressId,
                IsCompleted = false,
                IsActive = true,
            }
        );
        context.SaveChanges();

        var progressRepo = InMemoryQueryHelper.CreateUserCourseProgressRepo(context);
        _repositoryWrapperMock.Setup(x => x.UserCourseProgress).Returns(progressRepo);

        var progress = context.UserCourseProgress.Find(progressId)!;
        progress.IsCompleted = true;
        await _courseContentService.UpdateProgressAsync(progress, default);

        var updated = context.UserCourseProgress.Find(progressId);
        Assert.True(updated!.IsCompleted);
    }

    [Fact]
    public async Task UpdateEnrollmentProgressAsync_ShouldCallRepository()
    {
        var enrollmentId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Enrollment.Add(
            new Enrollment
            {
                Id = enrollmentId,
                ProgressPercentage = 0,
                IsActive = true,
            }
        );
        context.SaveChanges();

        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Enrollment).Returns(enrollmentRepo);

        await _courseContentService.UpdateEnrollmentProgressAsync(enrollmentId, 75m, default);

        var updated = context.Enrollment.Find(enrollmentId);
        Assert.Equal(75m, updated!.ProgressPercentage);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldCallSaveChanges()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var progressRepo = InMemoryQueryHelper.CreateUserCourseProgressRepo(context);
        _repositoryWrapperMock.Setup(x => x.UserCourseProgress).Returns(progressRepo);
        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _courseContentService.SaveChangesAsync(default);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
