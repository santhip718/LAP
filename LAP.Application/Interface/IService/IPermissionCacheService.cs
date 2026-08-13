namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides caching and retrieval of role-based permission sets.
/// </summary>
public interface IPermissionCacheService
{
    /// <summary>
    /// Gets the cached permission set for the specified role, or loads and caches it if not present.
    /// </summary>
    /// <param name="role">The role name to retrieve permissions for.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A set of feature names granted to the role.</returns>
    Task<HashSet<string>> GetPermissionsAsync(
        string role,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes the cached permission set for the specified role, forcing a reload on next access.
    /// </summary>
    /// <param name="role">The role name to invalidate cache for.</param>
    void RemoveRolePermissions(string role);
}
