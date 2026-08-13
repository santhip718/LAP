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

public class GetCourseProgressHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<GetCourseProgressHandler>> _loggerMock;
    private readonly GetCourseProgressHandler _handler;

    public GetCourseProgressHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<GetCourseProgressHandler>>();
        _handler = new GetCourseProgressHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnStoredProgress()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseProgressQuery(courseId);
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            ProgressPercentage = 50,
        };
        var dto = new CourseProgressResponseDto { ProgressPercentage = 50 };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.GetEnrollmentAsync(courseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);
        _mapperMock.Setup(x => x.Map<CourseProgressResponseDto>(enrollment)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(50, result.ProgressPercentage);
    }

    [Fact]
    public async Task Handle_ShouldReturnStoredProgress_WhenPartial()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseProgressQuery(courseId);
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            ProgressPercentage = 30,
        };
        var dto = new CourseProgressResponseDto { ProgressPercentage = 30 };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.GetEnrollmentAsync(courseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);
        _mapperMock.Setup(x => x.Map<CourseProgressResponseDto>(enrollment)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(30, result.ProgressPercentage);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNoContents()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseProgressQuery(courseId);
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            ProgressPercentage = 0,
        };
        var dto = new CourseProgressResponseDto { ProgressPercentage = 0 };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.GetEnrollmentAsync(courseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);
        _mapperMock.Setup(x => x.Map<CourseProgressResponseDto>(enrollment)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.ProgressPercentage);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNoneCompleted()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseProgressQuery(courseId);
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            ProgressPercentage = 0,
        };
        var dto = new CourseProgressResponseDto { ProgressPercentage = 0 };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.GetEnrollmentAsync(courseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);
        _mapperMock.Setup(x => x.Map<CourseProgressResponseDto>(enrollment)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.ProgressPercentage);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotEnrolled()
    {
        var query = new GetCourseProgressQuery(Guid.NewGuid());

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _courseServiceMock
            .Setup(x =>
                x.GetEnrollmentAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Enrollment?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Enrollment not found", ex.Message);
    }
}
