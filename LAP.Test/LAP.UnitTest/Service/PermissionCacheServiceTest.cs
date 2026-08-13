using System.Linq.Expressions;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Services;
using LAP.UnitTest.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace LAP.UnitTest.Service;

public class PermissionCacheServiceTest
{
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICustomLogger<PermissionCacheService>> _loggerMock;
    private readonly PermissionCacheService _service;

    public PermissionCacheServiceTest()
    {
        _cacheMock = new Mock<IMemoryCache>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<PermissionCacheService>>();
        _service = new PermissionCacheService(
            _repositoryWrapperMock.Object,
            _cacheMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnFromCache_WhenCacheHit()
    {
        var cachedPermissions = new HashSet<string> { "Feature1", "Feature2" };
        object cachedValue = cachedPermissions;
        _cacheMock.Setup(x => x.TryGetValue("permissions:Admin", out cachedValue!)).Returns(true);

        var result = await _service.GetPermissionsAsync("Admin");

        Assert.Equal(2, result.Count);
        Assert.Contains("Feature1", result);
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldLoadFromDb_WhenCacheMiss()
    {
        object? cachedValue = null;
        _cacheMock.Setup(x => x.TryGetValue("permissions:Admin", out cachedValue!)).Returns(false);
        _cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

        var roleId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var role = new RefTerm { Id = roleId, Name = "Admin" };
        var feature = new LAP.Domain.Entity.Feature { Id = featureId, Name = "ViewCourses" };
        var mapping = new RoleFeatureMapping
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            FeatureId = featureId,
            IsActive = true,
            Role = role,
            Feature = feature,
        };

        var mappings = new[] { mapping };

        _repositoryWrapperMock
            .Setup(x =>
                x.RoleFeatureMapping.FindByCondition(
                    It.IsAny<Expression<Func<RoleFeatureMapping, bool>>>()
                )
            )
            .Returns(
                (Expression<Func<RoleFeatureMapping, bool>> expr) =>
                    mappings.Where(expr.Compile()!).AsAsyncQueryable()
            );

        var result = await _service.GetPermissionsAsync("Admin");

        Assert.Contains("ViewCourses", result);
    }

    [Fact]
    public void RemoveRolePermissions_ShouldRemoveFromCache()
    {
        object? cachedValue = null;
        _cacheMock.Setup(x => x.Remove("permissions:Admin"));

        _service.RemoveRolePermissions("Admin");

        _cacheMock.Verify(x => x.Remove("permissions:Admin"), Times.Once);
    }
}
