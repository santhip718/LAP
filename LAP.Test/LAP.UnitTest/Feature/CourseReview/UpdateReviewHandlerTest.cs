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

public class UpdateReviewHandlerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<UpdateReviewHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateReviewHandler _handler;

    public UpdateReviewHandlerTest()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<UpdateReviewHandler>>();
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

        _handler = new UpdateReviewHandler(
            _reviewServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateReview()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new UpdateReviewRequestDto { Rating = 4, ReviewText = "Updated review" };
        var command = new UpdateReviewCommand(reviewId, dto);
        var review = new Review
        {
            Id = reviewId,
            UserId = userId,
            CourseId = Guid.NewGuid(),
            Rating = 3,
            ReviewText = "Old",
        };
        var reviewDto = new ReviewDto { Id = reviewId, Rating = 4 };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);
        _mapperMock.Setup(x => x.Map(dto, review));
        _reviewServiceMock
            .Setup(x => x.UpdateReviewAsync(review, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _transactionServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(review.CourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = review.CourseId });
        _reviewServiceMock
            .Setup(x => x.GetReviewByCourseIdAsync(review.CourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review> { review });
        _reviewServiceMock
            .Setup(x => x.UpdateCourseAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<ReviewDto>(review)).Returns(reviewDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(reviewId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReviewNotFound()
    {
        var command = new UpdateReviewCommand(Guid.NewGuid(), new UpdateReviewRequestDto());

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
        var command = new UpdateReviewCommand(reviewId, new UpdateReviewRequestDto { Rating = 4 });
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

    [Fact]
    public async Task Handle_ShouldThrow_WhenRatingOutOfRange()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateReviewCommand(reviewId, new UpdateReviewRequestDto { Rating = 6 });
        var review = new Review { Id = reviewId, UserId = userId };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _reviewServiceMock
            .Setup(x => x.GetReviewByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Invalid rating", ex.Message);
    }
}
