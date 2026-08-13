using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides caching and retrieval of reference data (reference sets and terms) for lookups.
/// </summary>
public interface IReferenceCacheService
{
    /// <summary>
    /// Gets all cached reference sets, or loads and caches them from the database if not present.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A read-only list of all reference sets.</returns>
    Task<IReadOnlyList<RefSet>> GetRefSetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all cached reference terms, or loads and caches them from the database if not present.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A read-only list of all reference terms.</returns>
    Task<IReadOnlyList<RefTerm>> GetRefTermAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cached reference sets, forcing a reload on next access.
    /// </summary>
    void ClearRefSetCache();

    /// <summary>
    /// Clears the cached reference terms, forcing a reload on next access.
    /// </summary>
    void ClearRefTermCache();
}
