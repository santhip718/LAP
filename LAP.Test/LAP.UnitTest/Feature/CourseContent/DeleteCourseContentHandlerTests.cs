using System;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.CourseContent;

public class DeleteCourseContentHandlerTests
{
    private readonly Mock<ICustomLogger<DeleteCourseContentHandler>> _loggerMock;
    private readonly Mock<ICourseContentService> _courseContentServiceMock;
    private readonly DeleteCourseContentHandler _handler;

    public DeleteCourseContentHandlerTests()
    {
        _loggerMock = new Mock<ICustomLogger<DeleteCourseContentHandler>>();
        _courseContentServiceMock = new Mock<ICourseContentService>();

        _handler = new DeleteCourseContentHandler(_loggerMock.Object, _courseContentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingContent_DeletesAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();

        _courseContentServiceMock
            .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new DeleteCourseContentCommand(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        _courseContentServiceMock.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
