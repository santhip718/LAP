using System;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Feature.Course.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Course;

public class DeleteCourseHandlerTests
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<ICustomLogger<DeleteCourseHandler>> _loggerMock;
    private readonly DeleteCourseHandler _handler;

    public DeleteCourseHandlerTests()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _loggerMock = new Mock<ICustomLogger<DeleteCourseHandler>>();

        _handler = new DeleteCourseHandler(_courseServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCourse_DeletesAndReturnsSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseServiceMock
            .Setup(s => s.DeleteCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new DeleteCourseCommand(courseId), CancellationToken.None);

        // Assert
        Assert.Equal(courseId, result.Id);
        _courseServiceMock.Verify(s => s.DeleteCourseAsync(courseId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
