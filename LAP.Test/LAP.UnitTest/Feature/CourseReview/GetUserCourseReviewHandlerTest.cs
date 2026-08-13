using AutoMapper;
using LAP.Application.DTO.Review;
using LAP.Application.Feature.CourseReview.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseReviewHandlers;

public class GetUserCourseReviewHandlerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<ICustomLogger<GetUserCourseReviewHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserCourseReviewHandler _handler;

    public GetUserCourseReviewHandlerTest()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _loggerMock = new Mock<ICustomLogger<GetUserCourseReviewHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserCourseReviewHandler(
            _reviewServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnReview()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetUserCourseReviewQuery(courseId, userId);
        var review = new Review { Id = Guid.NewGuid(), Rating = 4 };
        var reviewDto = new ReviewDto { Id = review.Id, Rating = 4 };

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetUserReviewForCourseAsync(courseId, userId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(review);
        _mapperMock.Setup(x => x.Map<ReviewDto>(review)).Returns(reviewDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(reviewDto.Id, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCourseNotFound()
    {
        var query = new GetUserCourseReviewQuery(Guid.NewGuid(), Guid.NewGuid());

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Course not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReviewNotFound()
    {
        var courseId = Guid.NewGuid();
        var query = new GetUserCourseReviewQuery(courseId, Guid.NewGuid());

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetUserReviewForCourseAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Review?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Review not found", ex.Message);
    }
}
