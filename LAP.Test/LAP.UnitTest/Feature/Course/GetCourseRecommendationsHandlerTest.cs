using AutoMapper;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseHandlers;

public class GetCourseRecommendationHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<GetCourseRecommendationHandler>> _loggerMock;
    private readonly GetCourseRecommendationHandler _handler;

    public GetCourseRecommendationHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<GetCourseRecommendationHandler>>();
        _handler = new GetCourseRecommendationHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnRecommendations()
    {
        var userId = Guid.NewGuid();
        var query = new GetCourseRecommendationQuery();
        var courses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Recommended Course" },
        };
        var dtos = new List<CourseSummaryDto>
        {
            new() { Id = courses[0].Id, Title = "Recommended Course" },
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.GetRecommendedCourseAsync(userId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);
        _mapperMock.Setup(x => x.Map<List<CourseSummaryDto>>(courses)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotAuthenticated()
    {
        var query = new GetCourseRecommendationQuery();

        _requestContextMock.Setup(x => x.UserId).Returns((Guid?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );
    }
}
