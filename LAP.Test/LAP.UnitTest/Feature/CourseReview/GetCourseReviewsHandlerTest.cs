using AutoMapper;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.Review;
using LAP.Application.Feature.CourseReview.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseReviewHandlers;

public class GetCourseReviewsHandlerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<ICustomLogger<GetCourseReviewsHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCourseReviewsHandler _handler;

    public GetCourseReviewsHandlerTest()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _loggerMock = new Mock<ICustomLogger<GetCourseReviewsHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCourseReviewsHandler(
            _reviewServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedReviews()
    {
        var courseId = Guid.NewGuid();
        var query = new GetCourseReviewsQuery(courseId, 1, 10);
        var reviews = new List<Review>
        {
            new Review { Id = Guid.NewGuid(), Rating = 5 },
        };
        var dtos = new List<ReviewDto>
        {
            new ReviewDto { Id = reviews[0].Id, Rating = 5 },
        };

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetPagedReviewsByCourseIdAsync(
                    courseId,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((reviews, 1));
        _mapperMock.Setup(x => x.Map<ICollection<ReviewDto>>(reviews)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.IsType<PaginatedReviewsDto>(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPaginationMetadata()
    {
        var courseId = Guid.NewGuid();
        var query = new GetCourseReviewsQuery(courseId, 2, 5);
        var reviews = new List<Review>
        {
            new Review { Id = Guid.NewGuid(), Rating = 5 },
            new Review { Id = Guid.NewGuid(), Rating = 4 },
        };
        var dtos = new List<ReviewDto>
        {
            new ReviewDto { Id = reviews[0].Id, Rating = 5 },
            new ReviewDto { Id = reviews[1].Id, Rating = 4 },
        };

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Course { Id = courseId });
        _reviewServiceMock
            .Setup(x =>
                x.GetPagedReviewsByCourseIdAsync(
                    courseId,
                    2,
                    5,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((reviews, 12));
        _mapperMock.Setup(x => x.Map<ICollection<ReviewDto>>(reviews)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(12, result.Total);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCourseNotFound()
    {
        var query = new GetCourseReviewsQuery(Guid.NewGuid());

        _reviewServiceMock
            .Setup(x => x.GetCourseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Course not found", ex.Message);
    }
}
