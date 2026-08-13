using AutoMapper;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseHandlers;

public class GetCourseOverviewHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<GetCourseOverviewHandler>> _loggerMock;
    private readonly GetCourseOverviewHandler _handler;

    public GetCourseOverviewHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<GetCourseOverviewHandler>>();
        _handler = new GetCourseOverviewHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCourseOverview()
    {
        var courseId = Guid.NewGuid();
        var query = new GetCourseOverviewQuery(courseId);
        var course = new Course { Id = courseId, Title = "Test Course" };
        var dto = new CourseOverviewDto { Title = "Test Course", Topic = new List<CourseOverviewMetaTopicDto>() };

        _courseServiceMock
            .Setup(x => x.GetCourseOverviewAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mapperMock.Setup(x => x.Map<CourseOverviewDto>(course)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal("Test Course", result.Title);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCourseNotFound()
    {
        var query = new GetCourseOverviewQuery(Guid.NewGuid());

        _courseServiceMock
            .Setup(x => x.GetCourseOverviewAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Course not found", ex.Message);
    }
}
