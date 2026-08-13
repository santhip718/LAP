using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Application.Service;
using LAP.Domain.Entity;
using Moq;
using System.Linq;
using MockQueryable.Moq;
using MockQueryable;
using Microsoft.EntityFrameworkCore;

namespace LAP.UnitTest.Services;

public class AuthServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<ICustomLogger<AuthService>> _loggerMock;
    private readonly IAuthService _authService;

    public AuthServiceTest()
    {
        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _userRepoMock = new Mock<IUserRepository>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _loggerMock = new Mock<ICustomLogger<AuthService>>();

        _repoWrapperMock.Setup(x => x.User).Returns(_userRepoMock.Object);
        _repoWrapperMock.Setup(x => x.RefreshToken).Returns(_refreshTokenRepoMock.Object);

        _authService = new AuthService(_repoWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        var email = "test@example.com";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            Person = new Person { Email = email },
            IsActive = true
        };
        var users = new List<User> { expectedUser }.BuildMock();

        _userRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .Returns(users);

        var result = await _authService.GetUserByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        var users = new List<User>().BuildMock();

        _userRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .Returns(users);

        var result = await _authService.GetUserByEmailAsync("missing@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        var personRepoMock = new Mock<IBaseRepository<Person>>();
        _repoWrapperMock.Setup(x => x.Repository<Person>()).Returns(personRepoMock.Object);

        personRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Person, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _authService.EmailExistsAsync("exists@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        var personRepoMock = new Mock<IBaseRepository<Person>>();
        _repoWrapperMock.Setup(x => x.Repository<Person>()).Returns(personRepoMock.Object);

        personRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Person, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _authService.EmailExistsAsync("new@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task AddPersonAsync_ShouldAddPersonAndSave()
    {
        var person = new Person { FullName = "John Doe", Email = "john@test.com" };
        var personRepoMock = new Mock<IBaseRepository<Person>>();
        _repoWrapperMock.Setup(x => x.Repository<Person>()).Returns(personRepoMock.Object);
        
        personRepoMock
            .Setup(x => x.CreateAsync(person, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _authService.AddPersonAsync(person);

        personRepoMock.Verify(x => x.CreateAsync(person, It.IsAny<CancellationToken>()), Times.Once);
        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserAndSave()
    {
        var user = new User { Id = Guid.NewGuid() };
        _userRepoMock
            .Setup(x => x.CreateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _authService.AddUserAsync(user);

        _userRepoMock.Verify(x => x.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserSecretAsync_ShouldAddSecret()
    {
        var secret = new UserSecret { UserId = Guid.NewGuid() };
        var secretRepoMock = new Mock<IBaseRepository<UserSecret>>();
        _repoWrapperMock.Setup(x => x.Repository<UserSecret>()).Returns(secretRepoMock.Object);
        
        secretRepoMock
            .Setup(x => x.CreateAsync(secret, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _authService.AddUserSecretAsync(secret);

        secretRepoMock.Verify(x => x.CreateAsync(secret, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserRoleMappingAsync_ShouldAddMapping()
    {
        var mapping = new UserRoleMapping { UserId = Guid.NewGuid() };
        var mappingRepoMock = new Mock<IBaseRepository<UserRoleMapping>>();
        _repoWrapperMock.Setup(x => x.Repository<UserRoleMapping>()).Returns(mappingRepoMock.Object);
        
        mappingRepoMock
            .Setup(x => x.CreateAsync(mapping, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _authService.AddUserRoleMappingAsync(mapping);

        mappingRepoMock.Verify(x => x.CreateAsync(mapping, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldDelegateToRepositoryWrapper()
    {
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _authService.SaveChangesAsync();

        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRoleNameByIdAsync_ShouldReturnName_WhenRoleExists()
    {
        var roleId = Guid.NewGuid();
        var role = new RefTerm { Id = roleId, Name = "Admin", IsActive = true };
        var roles = new List<RefTerm> { role }.BuildMock();

        var refRepoMock = new Mock<IBaseRepository<RefTerm>>();
        _repoWrapperMock.Setup(x => x.Repository<RefTerm>()).Returns(refRepoMock.Object);
        refRepoMock.Setup(x => x.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<RefTerm, bool>>>()))
            .Returns(roles);

        var result = await _authService.GetRoleNameByIdAsync(roleId);

        Assert.Equal("Admin", result);
    }

    [Fact]
    public async Task GetRoleNameByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        var roles = new List<RefTerm>().BuildMock();
        var refRepoMock = new Mock<IBaseRepository<RefTerm>>();
        _repoWrapperMock.Setup(x => x.Repository<RefTerm>()).Returns(refRepoMock.Object);
        refRepoMock.Setup(x => x.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<RefTerm, bool>>>()))
            .Returns(roles);

        var result = await _authService.GetRoleNameByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_ShouldReturnToken_WhenExists()
    {
        var tokenValue = "valid-token";
        var refreshToken = new RefreshToken { Token = tokenValue, IsActive = true };
        var tokens = new List<RefreshToken> { refreshToken }.BuildMock();

        _refreshTokenRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<RefreshToken, bool>>>()))
            .Returns(tokens);

        var result = await _authService.GetRefreshTokenAsync(tokenValue);

        Assert.NotNull(result);
        Assert.Equal(tokenValue, result.Token);
    }

    [Fact]
    public async Task AddRefreshTokenAsync_ShouldAddToken()
    {
        var token = new RefreshToken { UserId = Guid.NewGuid() };
        _refreshTokenRepoMock
            .Setup(x => x.CreateAsync(token, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _authService.AddRefreshTokenAsync(token);

        _refreshTokenRepoMock.Verify(x => x.CreateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldMarkRevokedAndSave()
    {
        var refreshToken = new RefreshToken { Token = "token", IsRevoked = false };
        _refreshTokenRepoMock.Setup(x => x.Update(refreshToken));
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _authService.RevokeRefreshTokenAsync(refreshToken);

        Assert.True(refreshToken.IsRevoked);
        _refreshTokenRepoMock.Verify(x => x.Update(refreshToken), Times.Once);
        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
