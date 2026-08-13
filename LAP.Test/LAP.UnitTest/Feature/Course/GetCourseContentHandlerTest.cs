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

public class GetCourseContentHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<GetCourseContentHandler>> _loggerMock;
    private readonly GetCourseContentHandler _handler;

    public GetCourseContentHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<GetCourseContentHandler>>();
        _handler = new GetCourseContentHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCourseContents()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseContentQuery(courseId);
        var course = new Course
        {
            Id = courseId,
            Topics = new List<CourseMetaTopic>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Topic 1",
                    Contents = new List<CourseContent>(),
                },
            },
        };
        var dto = new CourseContentResponseDto
        {
            CourseId = courseId,
            Topic = new List<CourseTopicProgressDto>(),
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x =>
                x.GetCourseWithProgressAsync(courseId, userId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(course);
        _mapperMock.Setup(x => x.Map<CourseContentResponseDto>(course)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(courseId, result.CourseId);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCourseNotFound()
    {
        var query = new GetCourseContentQuery(Guid.NewGuid());

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _courseServiceMock
            .Setup(x =>
                x.GetCourseWithProgressAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Course?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Course not found", ex.Message);
    }
}
