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

public class ReviewServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICustomLogger<ReviewService>> _loggerMock;
    private readonly ReviewService _reviewService;

    public ReviewServiceTest()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<ReviewService>>();
        _reviewService = new ReviewService(_repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetReviewByCourseIdAsync_ShouldReturnList()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var person = new Person { Id = Guid.NewGuid(), FullName = "Test User" };
        var user = new User
        {
            Id = userId,
            PersonId = person.Id,
            Person = person,
        };
        var context = InMemoryQueryHelper.CreateContext();
        context.Review.Add(
            new Review
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                UserId = userId,
                IsActive = true,
                User = user,
            }
        );
        context.SaveChanges();

        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        var result = await _reviewService.GetReviewByCourseIdAsync(courseId);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserReviewForCourseAsync_ShouldReturnReview()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var person = new Person { Id = Guid.NewGuid(), FullName = "Test User" };
        var user = new User
        {
            Id = userId,
            PersonId = person.Id,
            Person = person,
        };
        var context = InMemoryQueryHelper.CreateContext();
        context.Review.Add(
            new Review
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                UserId = userId,
                IsActive = true,
                User = user,
            }
        );
        context.SaveChanges();

        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        var result = await _reviewService.GetUserReviewForCourseAsync(courseId, userId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsUserEnrolledAsync_ShouldReturnTrue()
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
        _repositoryWrapperMock.Setup(x => x.Repository<Enrollment>()).Returns(enrollmentRepo);

        var result = await _reviewService.IsUserEnrolledAsync(userId, courseId);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserEnrollmentAsync_ShouldReturnEnrollment()
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
                EnrollmentStatus = true,
            }
        );
        context.SaveChanges();

        var enrollmentRepo = InMemoryQueryHelper.CreateEnrollmentRepo(context);
        _repositoryWrapperMock.Setup(x => x.Repository<Enrollment>()).Returns(enrollmentRepo);

        var result = await _reviewService.GetUserEnrollmentAsync(userId, courseId);

        Assert.NotNull(result);
        Assert.True(result.EnrollmentStatus);
    }

    [Fact]
    public async Task HasUserReviewedAsync_ShouldReturnFalse()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        var result = await _reviewService.HasUserReviewedAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourse()
    {
        var courseId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Course.Add(new Course { Id = courseId, IsActive = true });
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Repository<Course>()).Returns(courseRepo);

        var result = await _reviewService.GetCourseByIdAsync(courseId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldReturnReview()
    {
        var reviewId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Review.Add(new Review { Id = reviewId, IsActive = true });
        context.SaveChanges();

        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        var result = await _reviewService.GetReviewByIdAsync(reviewId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddReviewAsync_ShouldCallRepository()
    {
        var context = InMemoryQueryHelper.CreateContext();
        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);
        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var review = new Review { Id = Guid.NewGuid() };
        await _reviewService.AddReviewAsync(review);
        await context.SaveChangesAsync();

        Assert.Contains(review, context.Review.ToList());
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldUpdate()
    {
        var reviewId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Review.Add(
            new Review
            {
                Id = reviewId,
                ReviewText = "Original",
                IsActive = true,
            }
        );
        context.SaveChanges();

        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        var review = context.Review.Find(reviewId)!;
        review.ReviewText = "Updated";
        await _reviewService.UpdateReviewAsync(review);
        await context.SaveChangesAsync();

        var updated = context.Review.Find(reviewId);
        Assert.Equal("Updated", updated!.ReviewText);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldDelete()
    {
        var reviewId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Review.Add(new Review { Id = reviewId, IsActive = true });
        context.SaveChanges();

        var reviewRepo = InMemoryQueryHelper.CreateReviewRepo(context);
        _repositoryWrapperMock.Setup(x => x.Review).Returns(reviewRepo);

        await _reviewService.DeleteReviewAsync(reviewId);

        Assert.False(context.Review.Find(reviewId)!.IsActive);
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldUpdate()
    {
        var courseId = Guid.NewGuid();
        var context = InMemoryQueryHelper.CreateContext();
        context.Course.Add(
            new Course
            {
                Id = courseId,
                Title = "Original",
                IsActive = true,
            }
        );
        context.SaveChanges();

        var courseRepo = InMemoryQueryHelper.CreateCourseRepo(context);
        _repositoryWrapperMock.Setup(x => x.Repository<Course>()).Returns(courseRepo);

        var course = context.Course.Find(courseId)!;
        course.Title = "Updated";
        await _reviewService.UpdateCourseAsync(course);

        var updated = context.Course.Find(courseId);
        Assert.Equal("Updated", updated!.Title);
    }
}
