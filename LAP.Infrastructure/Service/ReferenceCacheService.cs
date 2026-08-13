using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LAP.Infrastructure.Services;

/// <summary>
/// Caches reference sets and terms in memory with a configurable expiration period.
/// </summary>
public class ReferenceCacheService : IReferenceCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<ReferenceCacheService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceCacheService"/> class.
    /// </summary>
    /// <param name="cache">The memory cache instance used to store reference data.</param>
    /// <param name="dbContext">The database context for loading reference data on cache miss.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public ReferenceCacheService(
        IMemoryCache cache,
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<ReferenceCacheService> logger
    )
    {
        _cache = cache;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all reference sets, returning cached data if available.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of all reference sets.</returns>
    public async Task<IReadOnlyList<RefSet>> GetRefSetAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (
            _cache.TryGetValue(
                CommonConstants.REF_SETS_CACHE_KEY,
                out IReadOnlyList<RefSet>? refSets
            )
        )
        {
            _logger.LogDebug("{Count} reference sets returned from cache.", refSets!.Count);
            return refSets;
        }

        refSets = await _repositoryWrapper
            .Repository<RefSet>()
            .FindByCondition(x => x.IsActive)
            .ToListAsync(cancellationToken);

        _cache.Set(CommonConstants.REF_SETS_CACHE_KEY, refSets, TimeSpan.FromHours(12));

        _logger.LogDebug(
            "ReferenceCacheService: {Count} reference sets loaded from database and cached",
            refSets.Count
        );

        return refSets;
    }

    /// <summary>
    /// Retrieves all reference terms, returning cached data if available.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of all reference terms.</returns>
    public async Task<IReadOnlyList<RefTerm>> GetRefTermAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (
            _cache.TryGetValue(
                CommonConstants.REF_TERMS_CACHE_KEY,
                out IReadOnlyList<RefTerm>? refTerms
            )
        )
        {
            _logger.LogDebug("{Count} reference terms returned from cache.", refTerms!.Count);
            return refTerms;
        }

        refTerms = await _repositoryWrapper
            .Repository<RefTerm>()
            .FindByCondition(x => x.IsActive)
            .ToListAsync(cancellationToken);

        _cache.Set(CommonConstants.REF_TERMS_CACHE_KEY, refTerms, TimeSpan.FromHours(12));

        _logger.LogDebug("{Count} reference terms loaded from database and cached", refTerms.Count);

        return refTerms;
    }

    /// <summary>
    /// Removes the reference sets cache entry so it is reloaded on the next request.
    /// </summary>
    public void ClearRefSetCache()
    {
        _cache.Remove(CommonConstants.REF_SETS_CACHE_KEY);
        _logger.LogDebug("Reference sets cache cleared.");
    }

    /// <summary>
    /// Removes the reference terms cache entry so it is reloaded on the next request.
    /// </summary>
    public void ClearRefTermCache()
    {
        _cache.Remove(CommonConstants.REF_TERMS_CACHE_KEY);
        _logger.LogDebug("Reference terms cache cleared.");
    }
}
