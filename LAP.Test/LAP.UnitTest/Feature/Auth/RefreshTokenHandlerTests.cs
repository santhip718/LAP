using LAP.Application.DTO;
using LAP.Application.DTO.Auth;
using LAP.Application.Feature.Auth.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Auth;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly Mock<ICustomLogger<RefreshTokenHandler>> _loggerMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _jwtHelperMock = new Mock<IJwtHelper>();
        _loggerMock = new Mock<ICustomLogger<RefreshTokenHandler>>();
        _jwtSettings = Options.Create(
            new JwtSettings
            {
                SecretKey = "test-secret-key-1234567890123456",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryInMinutes = 60,
                RefreshTokenExpiryInDays = 7,
            }
        );
        _handler = new RefreshTokenHandler(
            _authServiceMock.Object,
            _jwtHelperMock.Object,
            _loggerMock.Object,
            _jwtSettings
        );
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var tokenValue = "valid-refresh-token";
        var user = new LAP.Domain.Entity.User
        {
            Id = Guid.NewGuid(),
            Person = new Person { Email = "test@example.com", FullName = "Test User" },
            UserRoles = new List<UserRoleMapping>(),
        };
        var refreshToken = new RefreshToken
        {
            Token = tokenValue,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            User = user,
        };

        var requestDto = new RefreshRequestDto { RefreshToken = tokenValue };
        var command = new RefreshTokenCommand(requestDto);
        var expectedResponse = new AuthTokenResponseDto
        {
            AccessToken = "new-access",
            RefreshToken = "new-refresh",
        };

        _authServiceMock
            .Setup(s => s.GetRefreshTokenAsync(tokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        _jwtHelperMock
            .Setup(j =>
                j.GenerateToken(
                    user.Id,
                    user.Person.Email,
                    user.Person.FullName,
                    It.IsAny<List<string>>()
                )
            )
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse.AccessToken, result.AccessToken);
        Assert.Equal(expectedResponse.RefreshToken, result.RefreshToken);
        _authServiceMock.Verify(
            s => s.RevokeRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _authServiceMock.Verify(
            s => s.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _authServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var tokenValue = "expired-token";
        var refreshToken = new RefreshToken
        {
            Token = tokenValue,
            ExpiryDate = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false,
        };

        var requestDto = new RefreshRequestDto { RefreshToken = tokenValue };
        var command = new RefreshTokenCommand(requestDto);

        _authServiceMock
            .Setup(s => s.GetRefreshTokenAsync(tokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_RevokedToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var tokenValue = "revoked-token";
        var refreshToken = new RefreshToken
        {
            Token = tokenValue,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            IsRevoked = true,
        };

        var requestDto = new RefreshRequestDto { RefreshToken = tokenValue };
        var command = new RefreshTokenCommand(requestDto);

        _authServiceMock
            .Setup(s => s.GetRefreshTokenAsync(tokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }
}
