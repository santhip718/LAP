using System.Linq.Expressions;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using LAP.Infrastructure.Repository;
using LAP.Shared.Exceptions;
using LAP.UnitTest.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LAP.UnitTest.Service;

public class CourseServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICustomLogger<CourseService>> _loggerMock;
    private readonly CourseService _courseService;

    public CourseServiceTest()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<CourseService>>();
        _courseService = new CourseService(_repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPagedCoursesAsync_ShouldReturnPagedResult()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var category = new RefTerm { Id = Guid.NewGuid(), Name = "Tech" };
        var difficulty = new RefTerm { Id = Guid.NewGuid(), Name = "Beginner" };
        context.RefTerm.AddRange(category, difficulty);
        context.Course.Add(new Course
        {
            Id = Guid.NewGuid(),
            Title = "Course 1",
            IsActive = true,
            CategoryId = category.Id,
            DifficultyLevelId = difficulty.Id,
            Category = category,
            DifficultyLevel = difficulty,
        });
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var (items, totalCount) = await _courseService.GetPagedCoursesAsync(
            1,
            10,
            null,
            null,
            null,
            null
        );

        Assert.Single(items);
        Assert.Equal(1, totalCount);
    }

    [Fact]
    public async Task GetRecommendedCourseAsync_ShouldReturnCourses()
    {
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var recommendedCourseId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        var category = new RefTerm { Id = categoryId, Name = "Tech" };
        var difficulty = new RefTerm { Id = Guid.NewGuid(), Name = "Beginner" };
        context.RefTerm.AddRange(category, difficulty);
        var course = new Course
        {
            Id = courseId,
            Title = "Enrolled Course",
            IsActive = true,
            IsDrafted = false,
            CategoryId = categoryId,
            DifficultyLevelId = difficulty.Id,
            Category = category,
            DifficultyLevel = difficulty,
        };
        var recommendedCourse = new Course
        {
            Id = recommendedCourseId,
            Title = "Recommended",
            IsActive = true,
            IsDrafted = false,
            CategoryId = categoryId,
            DifficultyLevelId = difficulty.Id,
            Category = category,
            DifficultyLevel = difficulty,
        };
        context.Course.AddRange(course, recommendedCourse);
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            IsActive = true,
            Course = course,
        };

        _repositoryWrapperMock
            .Setup(x => x.Enrollment.FindByCondition(It.IsAny<Expression<Func<Enrollment, bool>>>()))
            .Returns((Expression<Func<Enrollment, bool>> expr) =>
                new[] { enrollment }.AsQueryable().Where(expr).AsAsyncQueryable());

        var result = await _courseService.GetRecommendedCourseAsync(userId, 5);

        var single = Assert.Single(result);
        Assert.Equal(recommendedCourseId, single.Id);
    }

    [Fact]
    public async Task GetCourseOverviewAsync_ShouldReturnCourse()
    {
        var courseId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        var category = new RefTerm { Id = Guid.NewGuid(), Name = "Tech" };
        var subCategory = new RefTerm { Id = Guid.NewGuid(), Name = "Programming" };
        var difficulty = new RefTerm { Id = Guid.NewGuid(), Name = "Beginner" };
        var person = new Person { Id = Guid.NewGuid(), FullName = "Author" };
        var user = new User { Id = Guid.NewGuid(), PersonId = person.Id, Person = person };
        context.RefTerm.AddRange(category, subCategory, difficulty);
        context.Person.Add(person);
        context.User.Add(user);
        context.Course.Add(new Course
        {
            Id = courseId,
            IsActive = true,
            CategoryId = category.Id,
            SubCategoryId = subCategory.Id,
            DifficultyLevelId = difficulty.Id,
            CreatedByUserId = user.Id,
            Category = category,
            SubCategory = subCategory,
            DifficultyLevel = difficulty,
            CreatedByUser = user,
        });
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var result = await _courseService.GetCourseOverviewAsync(courseId);

        Assert.Equal(courseId, result!.Id);
    }

    [Fact]
    public async Task GetCourseOverviewAsync_ShouldReturnNull_WhenNotFound()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var result = await _courseService.GetCourseOverviewAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCourseWithProgressAsync_ShouldReturnCourse()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();

        var metaTopic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            SequenceOrder = 1,
        };
        var contentType = new RefTerm { Id = Guid.NewGuid(), Name = "Video" };

        context.Course.Add(new Course
        {
            Id = courseId,
            IsActive = true,
            Topics = new List<CourseMetaTopic>
            {
                metaTopic,
            },
        });
        context.CourseMetaTopic.Add(metaTopic);
        context.CourseContent.Add(new CourseContent
        {
            Id = Guid.NewGuid(),
            MetaTopicId = metaTopic.Id,
            SequenceOrder = 1,
            ContentTypeId = contentType.Id,
            ContentType = contentType,
        });
        context.RefTerm.Add(contentType);
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var result = await _courseService.GetCourseWithProgressAsync(courseId, userId);

        Assert.Equal(courseId, result!.Id);
    }

    [Fact]
    public async Task GetEnrollmentAsync_ShouldReturnEnrollment()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Enrollment.Add(new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            IsActive = true,
        });
        context.SaveChanges();

        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Enrollment).Returns(enrollmentRepo);

        var result = await _courseService.GetEnrollmentAsync(courseId, userId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAssessmentHistoryAsync_ShouldReturnPagedResult()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();

        var assessment = new Assessment
        {
            Id = assessmentId,
            CourseId = courseId,
        };
        context.Assessment.Add(assessment);
        context.AssessmentHistory.Add(new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            AssessmentId = assessmentId,
            UserId = userId,
            IsActive = true,
            StartedOn = DateTime.UtcNow,
            Assessment = assessment,
        });
        context.SaveChanges();

        var historyRepo = InMemoryQueryHelper.CreateAssessmentHistoryRepo(context);
        _repositoryWrapperMock.Setup(x => x.AssessmentHistory).Returns(historyRepo);

        var result = await _courseService.GetAssessmentHistoryAsync(
            courseId,
            userId,
            1,
            10
        );

        Assert.Single(result.Item);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task RequestEnrollmentAsync_ShouldCreateEnrollment()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Course.Add(new Course { Id = courseId, IsActive = true });
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);
        _repositoryWrapperMock.Setup(x => x.Enrollment).Returns(enrollmentRepo);
        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _courseService.RequestEnrollmentAsync(courseId, userId);

        Assert.NotNull(result);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.EnrollmentStatus);
        Assert.Equal(0, result.ProgressPercentage);
    }

    [Fact]
    public async Task RequestEnrollmentAsync_ShouldReturnNull_WhenCourseNotFound()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);

        var result = await _courseService.RequestEnrollmentAsync(courseId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task RequestEnrollmentAsync_ShouldThrow_WhenAlreadyEnrolled()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();

        context.Enrollment.Add(new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            IsActive = true,
        });
        context.Course.Add(new Course { Id = courseId, IsActive = true });
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Course).Returns(courseRepo);
        _repositoryWrapperMock.Setup(x => x.Enrollment).Returns(enrollmentRepo);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            _courseService.RequestEnrollmentAsync(courseId, userId)
        );

        Assert.Equal("Already enrolled", ex.Message);
    }
}
