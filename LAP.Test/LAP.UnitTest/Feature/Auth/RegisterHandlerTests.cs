using FluentValidation;
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

public class RegisterHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly Mock<ICustomLogger<RegisterHandler>> _loggerMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _jwtHelperMock = new Mock<IJwtHelper>();
        _loggerMock = new Mock<ICustomLogger<RegisterHandler>>();
        _jwtSettings = Options.Create(new JwtSettings { RefreshTokenExpiryInDays = 7 });

        // Mock TransactionService to execute the operation immediately
        _transactionServiceMock.Setup(s => s.ExecuteInTransactionAsync(It.IsAny<Func<Task<AuthTokenResponseDto>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<AuthTokenResponseDto>>, CancellationToken>((operation, ct) => operation());

        _handler = new RegisterHandler(
            _authServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _jwtHelperMock.Object,
            _jwtSettings
        );
    }

    [Fact]
    public async Task Handle_NewUser_RegistersSuccessfullyAndReturnsTokens()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            FullName = "New User",
            Email = "new@example.com",
            Password = "Password123!",
            MobileNumber = "1234567890"
        };
        var command = new RegisterCommand(registerDto);
        var expectedResponse = new AuthTokenResponseDto { AccessToken = "access", RefreshToken = "refresh" };

        _authServiceMock.Setup(s => s.EmailExistsAsync(registerDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _authServiceMock.Setup(s => s.GetRoleNameByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Student");

        _jwtHelperMock.Setup(j => j.GenerateToken(It.IsAny<Guid>(), registerDto.Email, registerDto.FullName, It.IsAny<List<string>>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse.AccessToken, result.AccessToken);
        Assert.Equal(expectedResponse.RefreshToken, result.RefreshToken);
        _authServiceMock.Verify(s => s.AddPersonAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(s => s.AddUserAsync(It.IsAny<LAP.Domain.Entity.User>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(s => s.AddUserSecretAsync(It.IsAny<UserSecret>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(s => s.AddUserRoleMappingAsync(It.IsAny<UserRoleMapping>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(s => s.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }


}

public class RegisterValidatorTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly RegisterValidator _validator;

    public RegisterValidatorTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _validator = new RegisterValidator(_authServiceMock.Object);
    }

    [Fact]
    public async Task ShouldNotError_WhenValidEmailAndPassword()
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password1!",
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalidemail")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    public async Task ShouldError_WhenEmailIsInvalid(string email)
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = email,
                Password = "Password1!",
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.PropertyName == "Dto.Email"
        );
    }

    [Fact]
    public async Task ShouldError_WhenEmailAlreadyExists()
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "existing@example.com",
                Password = "Password1!",
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage.Contains("Email already exists")
        );
    }

    [Theory]
    [InlineData("password")]
    [InlineData("PASSWORD")]
    [InlineData("12345678")]
    [InlineData("abcdefgh")]
    [InlineData("Abcdefg1")]
    public async Task ShouldError_WhenPasswordLacksComplexity(string password)
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = password,
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.PropertyName == "Dto.Password"
        );
    }

    [Theory]
    [InlineData("Password1!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Str0ng!Pass")]
    public async Task ShouldNotError_WhenPasswordMeetsComplexity(string password)
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = password,
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ShouldError_WhenPasswordTooShort()
    {
        _authServiceMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(
            new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Ab1!",
                MobileNumber = "1234567890",
            }
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage.Contains("at least 8 characters")
        );
    }
}
