using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.UnitTest.Helpers;
using Moq;
using Xunit;

namespace LAP.UnitTest.Service;

public class UserServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICustomLogger<UserService>> _loggerMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _userRepoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ICustomLogger<UserService>>();

        _repoMock.Setup(r => r.User).Returns(_userRepoMock.Object);

        _service = new UserService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUserWithDetailAsync_ShouldReturnList()
    {
        var users = new List<User> { new User() };
        _userRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(users.AsAsyncQueryable());

        var result = await _service.GetAllUserWithDetailAsync();

        Assert.Equal(users, result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userRepoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetUserByIdAsync(id);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByIdWithDetailAsync_ShouldReturnUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsAsyncQueryable());

        var result = await _service.GetUserByIdWithDetailAsync(id);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByIdWithSecretAsync_ShouldReturnUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsAsyncQueryable());

        var result = await _service.GetUserByIdWithSecretAsync(id);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByIdWithEnrollmentsAsync_ShouldReturnUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsAsyncQueryable());

        var result = await _service.GetUserByIdWithEnrollmentsAsync(id);

        Assert.Equal(user, result);
    }

    [Fact]
    public void UpdateUser_ShouldCallRepo()
    {
        var user = new User { Id = Guid.NewGuid() };

        _service.UpdateUser(user);

        _userRepoMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdWithPersonAsync_ShouldReturnUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Person = new Person() };
        _userRepoMock
            .Setup(r => r.FindByConditionWithTracking(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsAsyncQueryable());

        var result = await _service.GetUserByIdWithPersonAsync(id);

        Assert.Equal(user, result);
    }
}
