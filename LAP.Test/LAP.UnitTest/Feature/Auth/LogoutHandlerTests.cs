using LAP.Application.Feature.Auth.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Auth;

public class LogoutHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ICustomLogger<LogoutHandler>> _loggerMock;
    private readonly LogoutHandler _handler;

    public LogoutHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ICustomLogger<LogoutHandler>>();
        _handler = new LogoutHandler(_authServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingToken_RevokesTokenAndReturnsSuccessResponse()
    {
        // Arrange
        var token = "valid-token";
        var command = new LogoutCommand(token);
        var refreshToken = new RefreshToken { Token = token };

        _authServiceMock.Setup(s => s.GetRefreshTokenAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Logged out successfully", result.Message);
        _authServiceMock.Verify(s => s.RevokeRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingToken_ReturnsSuccessResponse()
    {
        // Arrange
        var token = "invalid-token";
        var command = new LogoutCommand(token);

        _authServiceMock.Setup(s => s.GetRefreshTokenAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Logged out successfully", result.Message);
        _authServiceMock.Verify(s => s.RevokeRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
