using LAP.Application.Feature.Course.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Features.CourseHandlers;

public class RequestEnrollmentHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<RequestEnrollmentHandler>> _loggerMock;
    private readonly RequestEnrollmentHandler _handler;

    public RequestEnrollmentHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<RequestEnrollmentHandler>>();
        _handler = new RequestEnrollmentHandler(
            _courseServiceMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateEnrollment()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RequestEnrollmentCommand(courseId);
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x => x.RequestEnrollmentAsync(courseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(enrollment.Id, result.Id);
        Assert.Equal("Enrollment request created", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldUseEmptyGuid_WhenUserNotAuthenticated()
    {
        var courseId = Guid.NewGuid();
        var command = new RequestEnrollmentCommand(courseId);
        var enrollment = new Enrollment { Id = Guid.NewGuid() };

        _requestContextMock.Setup(x => x.UserId).Returns((Guid?)null);
        _courseServiceMock
            .Setup(x =>
                x.RequestEnrollmentAsync(courseId, Guid.Empty, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(enrollment);

        await _handler.Handle(command, CancellationToken.None);

        _courseServiceMock.Verify(
            x => x.RequestEnrollmentAsync(courseId, Guid.Empty, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
