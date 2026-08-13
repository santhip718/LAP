using LAP.Application.DTO;
using LAP.Application.DTO.Auth;
using LAP.Application.Feature.Auth.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using LAP.Shared.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Auth;

public class LoginHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ICustomLogger<LoginHandler>> _loggerMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ICustomLogger<LoginHandler>>();
        _jwtHelperMock = new Mock<IJwtHelper>();
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
        _handler = new LoginHandler(
            _authServiceMock.Object,
            _loggerMock.Object,
            _jwtHelperMock.Object,
            _jwtSettings
        );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var password = "Password123!";
        var email = "test@example.com";
        var hash = UserSecretHelper.HashPasswordBcrypt(password, out string salt);
        
        var user = new LAP.Domain.Entity.User
        {
            Id = Guid.NewGuid(),
            Person = new Person { Email = email, FullName = "Test User" },
            UserSecret = new UserSecret { PasswordHash = hash },
            UserRoles = new List<UserRoleMapping>()
        };

        var loginDto = new LoginRequestDto { Email = email, Password = password };
        var command = new LoginCommand(loginDto);
        var expectedResponse = new AuthTokenResponseDto { AccessToken = "access", RefreshToken = "refresh" };

        _authServiceMock.Setup(s => s.GetUserByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _jwtHelperMock.Setup(j => j.GenerateToken(user.Id, user.Person.Email, user.Person.FullName, It.IsAny<List<string>>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse.AccessToken, result.AccessToken);
        Assert.Equal(expectedResponse.RefreshToken, result.RefreshToken);
        _authServiceMock.Verify(s => s.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginRequestDto { Email = "notfound@example.com", Password = "password" };
        var command = new LoginCommand(loginDto);

        _authServiceMock.Setup(s => s.GetUserByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var email = "test@example.com";
        var user = new LAP.Domain.Entity.User
        {
            Id = Guid.NewGuid(),
            Person = new Person { Email = email },
            UserSecret = new UserSecret { PasswordHash = UserSecretHelper.HashPasswordBcrypt("correct", out _) }
        };

        var loginDto = new LoginRequestDto { Email = email, Password = "wrong" };
        var command = new LoginCommand(loginDto);

        _authServiceMock.Setup(s => s.GetUserByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
