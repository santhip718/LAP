using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LAP.Infrastructure.Services;

/// <summary>
/// Caches role-based permission sets in memory with a one-hour expiration.
/// </summary>
public class PermissionCacheService : IPermissionCacheService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMemoryCache _cache;
    private readonly ICustomLogger<PermissionCacheService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionCacheService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public PermissionCacheService(
        IRepositoryWrapper repositoryWrapper,
        IMemoryCache cache,
        ICustomLogger<PermissionCacheService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Gets the cached permission set for the specified role, loading from database on cache miss.
    /// </summary>
    /// <param name="role">The role name to retrieve permissions for.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A set of feature names granted to the role.</returns>
    public async Task<HashSet<string>> GetPermissionsAsync(
        string role,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving permissions for role {Role}.", role);
        string cacheKey = $"permissions:{role}";

        if (_cache.TryGetValue(cacheKey, out HashSet<string>? permissionSet))
        {
            _logger.LogDebug("Retrieved permissions from cache for role {Role}.", role);
            return permissionSet!;
        }

        IEnumerable<string> permissionFromDb = await _repositoryWrapper
            .RoleFeatureMapping.FindByCondition(x => x.IsActive && x.Role.Name == role)
            .Select(x => x.Feature.Name)
            .ToListAsync(cancellationToken);

        permissionSet = permissionFromDb.ToHashSet();

        _cache.Set(
            cacheKey,
            permissionSet,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
        );

        _logger.LogDebug("Retrieved permissions from database for role {Role}.", role);
        return permissionSet;
    }

    /// <summary>
    /// Removes the cached permission set for the specified role, forcing a reload on next access.
    /// </summary>
    /// <param name="role">The role name to invalidate cache for.</param>
    public void RemoveRolePermissions(string role)
    {
        _logger.LogDebug("Removing permissions for role {Role}.", role);
        _cache.Remove($"permissions:{role}");
        _logger.LogDebug("Removed permissions for role {Role}.", role);
    }
}
