using LAP.Application.DTO;
using LAP.Application.Helpers;
using LAP.Application.Interface;
using Microsoft.Extensions.Options;
using Moq;

namespace LAP.UnitTest.Service;

public class JwtHelperTest
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtHelper _jwtHelper;
    private readonly Mock<ICustomLogger<JwtHelper>> _loggerMock;

    public JwtHelperTest()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsASecretKeyForTestingPurposes1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryInMinutes = 60,
            RefreshTokenExpiryInDays = 7,
        };
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        optionsMock.Setup(x => x.Value).Returns(_jwtSettings);
        _loggerMock = new Mock<ICustomLogger<JwtHelper>>();
        _jwtHelper = new JwtHelper(optionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnTokenWithCorrectExpiry()
    {
        var userId = Guid.NewGuid();
        var roles = new List<string> { "Student" };

        var result = _jwtHelper.GenerateToken(userId, "test@test.com", "Test User", roles);

        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public void GenerateToken_ShouldIncludeRoleClaims()
    {
        var userId = Guid.NewGuid();
        var roles = new List<string> { "Admin", "Student" };

        var result = _jwtHelper.GenerateToken(userId, "admin@test.com", "Admin User", roles);

        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public void GenerateToken_ShouldHandleNoRoles()
    {
        var userId = Guid.NewGuid();
        var roles = new List<string>();

        var result = _jwtHelper.GenerateToken(userId, "test@test.com", "Test User", roles);

        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public void GenerateToken_WithDifferentSettings_ShouldUseCustomExpiry()
    {
        var customSettings = new JwtSettings
        {
            SecretKey = "AnotherSecretKeyForTestingPurposes1234567890123456",
            Issuer = "CustomIssuer",
            Audience = "CustomAudience",
            ExpiryInMinutes = 30,
            RefreshTokenExpiryInDays = 1,
        };
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        optionsMock.Setup(x => x.Value).Returns(customSettings);
        var helper = new JwtHelper(optionsMock.Object, _loggerMock.Object);

        var result = helper.GenerateToken(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            new List<string>()
        );

        Assert.Equal(1800, result.ExpiresIn);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        var result = _jwtHelper.GenerateRefreshToken();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueTokens()
    {
        var token1 = _jwtHelper.GenerateRefreshToken();
        var token2 = _jwtHelper.GenerateRefreshToken();

        Assert.NotEqual(token1, token2);
    }
}
