using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.DTO.Forum;
using LAP.Application.Feature.Forum.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Forum;

public class CreateForumMessageHandlerTests
{
    private readonly Mock<IForumService> _forumServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<CreateForumMessageHandler>> _loggerMock;
    private readonly CreateForumMessageHandler _handler;

    public CreateForumMessageHandlerTests()
    {
        _forumServiceMock = new Mock<IForumService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<CreateForumMessageHandler>>();

        _transactionServiceMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>, CancellationToken>(async (op, ct) => await op());

        _handler = new CreateForumMessageHandler(
            _forumServiceMock.Object,
            _transactionServiceMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesMessage()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new CreateForumMessageRequestDto { MessageText = "Hello" };

        _forumServiceMock.Setup(s => s.CourseExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _requestContextMock.Setup(r => r.UserId).Returns(userId);

        // Act
        var result = await _handler.Handle(new CreateForumMessageCommand(courseId, dto), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        _forumServiceMock.Verify(s => s.AddMessageAsync(It.IsAny<ForumMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _forumServiceMock.Setup(s => s.CourseExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new CreateForumMessageCommand(courseId, new CreateForumMessageRequestDto()), CancellationToken.None));
    }
}
