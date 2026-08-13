using System;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Feature.User.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.User;

public class DeleteUserHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ICustomLogger<DeleteUserHandler>> _loggerMock;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ICustomLogger<DeleteUserHandler>>();

        _handler = new DeleteUserHandler(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_DeletesAndReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock
            .Setup(s => s.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Assert
        Assert.Equal(userId, result.Id);
        _userServiceMock.Verify(s => s.DeleteUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
