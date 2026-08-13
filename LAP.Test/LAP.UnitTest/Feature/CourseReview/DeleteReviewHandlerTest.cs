using LAP.Application.DTO.Common;
using LAP.Application.Feature.CourseReview.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseReviewHandlers;

public class DeleteReviewHandlerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<DeleteReviewHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly DeleteReviewHandler _handler;

    public DeleteReviewHandlerTest()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<DeleteReviewHandler>>();
        _requestContextMock = new Mock<IRequestContext>();

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((Func<Task> op, CancellationToken _) => op());

        _handler = new DeleteReviewHandler(
            _reviewServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldDeleteReview()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteReviewCommand(reviewId);
        var review = new Review
        {
            Id = reviewId,
            UserId = userId,
            CourseId = Guid.NewGuid(),
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);
        _reviewServiceMock
            .Setup(x => x.DeleteReviewAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _transactionServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(review.CourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = review.CourseId });
        _reviewServiceMock
            .Setup(x => x.GetReviewByCourseIdAsync(review.CourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review>());
        _reviewServiceMock
            .Setup(x => x.UpdateCourseAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(reviewId, result.Id);
        Assert.Equal("Review deleted successfully", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAffectedRowsZero()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteReviewCommand(reviewId);
        var review = new Review
        {
            Id = reviewId,
            UserId = userId,
            CourseId = Guid.NewGuid(),
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);
        _reviewServiceMock
            .Setup(x => x.DeleteReviewAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Review not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReviewNotFound()
    {
        var command = new DeleteReviewCommand(Guid.NewGuid());

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Review not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotOwner()
    {
        var reviewId = Guid.NewGuid();
        var command = new DeleteReviewCommand(reviewId);
        var review = new Review { Id = reviewId, UserId = Guid.NewGuid() };

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Access denied", ex.Message);
    }
}
