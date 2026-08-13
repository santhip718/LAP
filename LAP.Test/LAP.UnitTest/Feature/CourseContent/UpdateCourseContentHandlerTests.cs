using System;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.CourseContent;

public class UpdateCourseContentHandlerTests
{
    private readonly Mock<ICustomLogger<UpdateCourseContentHandler>> _loggerMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICourseContentService> _courseContentServiceMock;
    private readonly UpdateCourseContentHandler _handler;

    public UpdateCourseContentHandlerTests()
    {
        _loggerMock = new Mock<ICustomLogger<UpdateCourseContentHandler>>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _fileServiceMock = new Mock<IFileService>();
        _requestContextMock = new Mock<IRequestContext>();
        _courseContentServiceMock = new Mock<ICourseContentService>();

        _transactionServiceMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>, CancellationToken>(async (op, ct) => await op());

        _handler = new UpdateCourseContentHandler(
            _loggerMock.Object,
            _transactionServiceMock.Object,
            _fileServiceMock.Object,
            _requestContextMock.Object,
            _courseContentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingContent_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateCourseContentRequestDto { CourseId = Guid.NewGuid(), MetaTopic = "Topic", Title = "New Title" };
        var content = new LAP.Domain.Entity.CourseContent { Id = id, Title = "Old Title" };
        var metaTopic = new CourseMetaTopic { Id = Guid.NewGuid(), Name = "Topic" };

        _courseContentServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock.Setup(s => s.GetMetaTopicByCourseAndNameAsync(dto.CourseId, "Topic", It.IsAny<CancellationToken>()))
            .ReturnsAsync(metaTopic);

        // Act
        var result = await _handler.Handle(new UpdateCourseContentCommand(id, dto), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(dto.Title, content.Title);
        _courseContentServiceMock.Verify(s => s.Update(content), Times.Once);
        _transactionServiceMock.Verify(s => s.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingContent_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _courseContentServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.CourseContent?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new UpdateCourseContentCommand(id, new UpdateCourseContentRequestDto()), CancellationToken.None));
    }
}
