using AutoMapper;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.Paginated;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Features.CourseHandlers;

public class GetCourseQueryHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<GetCourseQueryHandler>> _loggerMock;
    private readonly GetCourseQueryHandler _handler;

    public GetCourseQueryHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<GetCourseQueryHandler>>();
        _handler = new GetCourseQueryHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedCourses()
    {
        var query = new GetCourseQuery(1, 10, null, null, null, null);
        var courses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Course 1" },
        };
        var dtos = new List<CourseSummaryDto>
        {
            new() { Id = courses[0].Id, Title = "Course 1" },
        };

        _courseServiceMock
            .Setup(x =>
                x.GetPagedCoursesAsync(1, 10, null, null, null, null, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((courses, 1));
        _mapperMock.Setup(x => x.Map<ICollection<CourseSummaryDto>>(courses)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoCourses()
    {
        var query = new GetCourseQuery(1, 10, null, null, null, null);

        _courseServiceMock
            .Setup(x =>
                x.GetPagedCoursesAsync(1, 10, null, null, null, null, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((new List<Course>(), 0));
        _mapperMock
            .Setup(x => x.Map<ICollection<CourseSummaryDto>>(It.IsAny<IEnumerable<Course>>()))
            .Returns(new List<CourseSummaryDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Data);
    }
}
