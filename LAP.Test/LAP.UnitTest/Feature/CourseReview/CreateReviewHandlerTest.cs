using AutoMapper;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Review;
using LAP.Application.Feature.CourseReview.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseReviewHandlers;

public class CreateReviewHandlerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<CreateReviewHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateReviewHandler _handler;

    public CreateReviewHandlerTest()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<CreateReviewHandler>>();
        _requestContextMock = new Mock<IRequestContext>();
        _mapperMock = new Mock<IMapper>();

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<ReviewDto>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((Func<Task<ReviewDto>> op, CancellationToken _) => op());

        _handler = new CreateReviewHandler(
            _reviewServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateReview()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new CreateReviewRequestDto { Rating = 5, ReviewText = "Great course!" };
        var command = new CreateReviewCommand(courseId, dto);
        var course = new Course { Id = courseId };
        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = 5,
            ReviewText = "Great course!",
        };
        var reviewDto = new ReviewDto { Id = review.Id, Rating = 5 };

        var approvedEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrollmentStatus = true,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _reviewServiceMock
            .Setup(x =>
                x.GetUserEnrollmentAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(approvedEnrollment);
        _reviewServiceMock
            .Setup(x => x.HasUserReviewedAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock.Setup(x => x.Map<Review>(dto)).Returns(review);
        _reviewServiceMock
            .Setup(x => x.AddReviewAsync(review, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _transactionServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _reviewServiceMock
            .Setup(x => x.GetReviewByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review> { review });
        _reviewServiceMock
            .Setup(x => x.UpdateCourseAsync(course, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<ReviewDto>(review)).Returns(reviewDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(reviewDto.Id, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCourseNotFound()
    {
        var command = new CreateReviewCommand(
            Guid.NewGuid(),
            new CreateReviewRequestDto { Rating = 5 }
        );

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Course not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotEnrolled()
    {
        var courseId = Guid.NewGuid();
        var command = new CreateReviewCommand(courseId, new CreateReviewRequestDto { Rating = 5 });

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetUserEnrollmentAsync(It.IsAny<Guid>(), courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Enrollment?)null);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Not enrolled", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEnrollmentPending()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateReviewCommand(courseId, new CreateReviewRequestDto { Rating = 5 });
        var pendingEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrollmentStatus = false,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetUserEnrollmentAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(pendingEnrollment);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Enrollment not approved", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAlreadyReviewed()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateReviewCommand(courseId, new CreateReviewRequestDto { Rating = 5 });
        var approvedEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrollmentStatus = true,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetUserEnrollmentAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(approvedEnrollment);
        _reviewServiceMock
            .Setup(x =>
                x.HasUserReviewedAsync(It.IsAny<Guid>(), courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Review already exists", ex.Message);
    }
}
