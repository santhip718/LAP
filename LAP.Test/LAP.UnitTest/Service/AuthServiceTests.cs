using System.Linq.Expressions;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.UnitTest.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace LAP.UnitTest.Service;

public class AuthServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<IBaseRepository<Person>> _personRepoMock;
    private readonly Mock<IBaseRepository<UserSecret>> _userSecretRepoMock;
    private readonly Mock<IBaseRepository<UserRoleMapping>> _userRoleMappingRepoMock;
    private readonly Mock<IBaseRepository<RefTerm>> _refTermRepoMock;
    private readonly Mock<ICustomLogger<AuthService>> _loggerMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _userRepoMock = new Mock<IUserRepository>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _personRepoMock = new Mock<IBaseRepository<Person>>();
        _userSecretRepoMock = new Mock<IBaseRepository<UserSecret>>();
        _userRoleMappingRepoMock = new Mock<IBaseRepository<UserRoleMapping>>();
        _refTermRepoMock = new Mock<IBaseRepository<RefTerm>>();
        _loggerMock = new Mock<ICustomLogger<AuthService>>();

        _repoMock.Setup(r => r.User).Returns(_userRepoMock.Object);
        _repoMock.Setup(r => r.RefreshToken).Returns(_refreshTokenRepoMock.Object);
        _repoMock.Setup(r => r.Repository<Person>()).Returns(_personRepoMock.Object);
        _repoMock.Setup(r => r.Repository<UserSecret>()).Returns(_userSecretRepoMock.Object);
        _repoMock.Setup(r => r.Repository<UserRoleMapping>()).Returns(_userRoleMappingRepoMock.Object);
        _repoMock.Setup(r => r.Repository<RefTerm>()).Returns(_refTermRepoMock.Object);

        _service = new AuthService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
    {
        var email = "test@test.com";
        var user = new User
        {
            Person = new Person { Email = email },
            UserSecret = new UserSecret(),
            UserRoles = new List<UserRoleMapping>
            {
                new() { Role = new RefTerm { Name = "Student" } },
            },
        };
        var users = new[] { user };

        _userRepoMock
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((Expression<Func<User, bool>> expr) => users.AsQueryable().Where(expr).AsAsyncQueryable());

        var result = await _service.GetUserByEmailAsync(email);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var email = "test@test.com";
        _personRepoMock
            .Setup(r => r.AnyByConditionAsync(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.EmailExistsAsync(email);

        Assert.True(result);
    }

    [Fact]
    public async Task AddPersonAsync_ShouldCallRepoAndSave()
    {
        var person = new Person { Id = Guid.NewGuid() };

        await _service.AddPersonAsync(person);

        _personRepoMock.Verify(r => r.CreateAsync(person, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserAsync_ShouldCallRepoAndSave()
    {
        var user = new User { Id = Guid.NewGuid() };

        await _service.AddUserAsync(user);

        _userRepoMock.Verify(r => r.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserSecretAsync_ShouldCallRepo()
    {
        var userSecret = new UserSecret { UserId = Guid.NewGuid() };

        await _service.AddUserSecretAsync(userSecret);

        _userSecretRepoMock.Verify(r => r.CreateAsync(userSecret, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUserRoleMappingAsync_ShouldCallRepo()
    {
        var mapping = new UserRoleMapping { UserId = Guid.NewGuid() };

        await _service.AddUserRoleMappingAsync(mapping);

        _userRoleMappingRepoMock.Verify(r => r.CreateAsync(mapping, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldCallRepo()
    {
        await _service.SaveChangesAsync();

        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRoleNameByIdAsync_ShouldReturnName()
    {
        var id = Guid.NewGuid();
        var name = "Admin";
        var refTerms = new[] { new RefTerm { Id = id, Name = name } };

        _refTermRepoMock
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<RefTerm, bool>>>()))
            .Returns((Expression<Func<RefTerm, bool>> expr) => refTerms.AsQueryable().Where(expr).AsAsyncQueryable());

        var result = await _service.GetRoleNameByIdAsync(id);

        Assert.Equal(name, result);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_ShouldReturnToken()
    {
        var tokenValue = "token";
        var refreshToken = new RefreshToken
        {
            Token = tokenValue,
            User = new User
            {
                Person = new Person(),
                UserRoles = new List<UserRoleMapping>
                {
                    new() { Role = new RefTerm { Name = "Admin" } },
                },
            },
        };
        var tokens = new[] { refreshToken };

        _refreshTokenRepoMock
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .Returns((Expression<Func<RefreshToken, bool>> expr) => tokens.AsQueryable().Where(expr).AsAsyncQueryable());

        var result = await _service.GetRefreshTokenAsync(tokenValue);

        Assert.Equal(refreshToken, result);
    }

    [Fact]
    public async Task AddRefreshTokenAsync_ShouldCallRepo()
    {
        var token = new RefreshToken();

        await _service.AddRefreshTokenAsync(token);

        _refreshTokenRepoMock.Verify(r => r.CreateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldUpdate()
    {
        var token = new RefreshToken { IsRevoked = false };

        await _service.RevokeRefreshTokenAsync(token);

        Assert.True(token.IsRevoked);
        _refreshTokenRepoMock.Verify(r => r.Update(token), Times.Once);
    }
}
