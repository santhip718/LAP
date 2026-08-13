using System.Collections.Concurrent;
using LAP.Application.Authorization;
using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LAP.API.Authorization;

/// <summary>
/// Dynamically resolves authorization policies by checking feature existence in the database and caching resolved policies.
/// </summary>
public class DynamicPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomLogger<DynamicPolicyProvider> _logger;

    private static readonly ConcurrentDictionary<string, AuthorizationPolicy?> _policyCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPolicyProvider"/> class.
    /// </summary>
    /// <param name="options">The authorization options.</param>
    /// <param name="serviceProvider">The service provider for creating scoped database contexts.</param>
    /// <param name="logger">The custom logger instance.</param>
    public DynamicPolicyProvider(
        IOptions<AuthorizationOptions> options,
        IServiceProvider serviceProvider,
        ICustomLogger<DynamicPolicyProvider> logger
    )
        : base(options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Gets an authorization policy by name. Checks the cache first, then attempts to create a feature-based policy or falls back to the default.
    /// </summary>
    /// <param name="policyName">The name of the policy to resolve.</param>
    /// <returns>The authorization policy if found; otherwise, <c>null</c>.</returns>
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        _logger.LogDebug("Resolving authorization policy. Policy name: {PolicyName}", policyName);

        if (string.IsNullOrWhiteSpace(policyName))
        {
            _logger.LogError("Policy name is null or empty. Falling back to base policy provider.");

            return await base.GetPolicyAsync(policyName);
        }

        if (_policyCache.TryGetValue(policyName, out AuthorizationPolicy? cachedPolicy))
        {
            _logger.LogDebug("Policy cache hit. Policy name: {PolicyName}", policyName);

            return cachedPolicy;
        }

        _logger.LogDebug("Policy cache miss. Policy name: {PolicyName}", policyName);

        AuthorizationPolicy? policy = null;

        if (policyName.StartsWith(CommonConstants.PolicyPrefix))
        {
            string featureName = policyName[CommonConstants.PolicyPrefix.Length..];

            _logger.LogDebug(
                "Feature-based policy detected. Feature name: {FeatureName}",
                featureName
            );

            policy = await CreateFeaturePolicyAsync(featureName);
        }
        else
        {
            _logger.LogDebug(
                "Attempting feature lookup for policy. Policy name: {PolicyName}",
                policyName
            );

            policy =
                await CreateFeaturePolicyAsync(policyName) ?? await base.GetPolicyAsync(policyName);

            if (policy != null)
            {
                _logger.LogDebug(
                    "Policy resolved successfully. Policy name: {PolicyName}",
                    policyName
                );
            }
            else
            {
                _logger.LogError(
                    "Policy could not be resolved. Policy name: {PolicyName}",
                    policyName
                );
            }
        }

        if (policy == null)
        {
            policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        }

        _policyCache[policyName] = policy;

        _logger.LogDebug("Policy cached. Policy name: {PolicyName}", policyName);

        return policy;
    }

    /// <summary>
    /// Creates an authorization policy requiring the specified feature, if the feature exists in the database.
    /// </summary>
    /// <param name="featureName">The name of the feature to require.</param>
    /// <returns>
    /// An authorization policy with a <see cref="FeatureRequirement"/>,
    /// or <c>null</c> if the feature does not exist.
    /// </returns>
    private async Task<AuthorizationPolicy?> CreateFeaturePolicyAsync(string featureName)
    {
        _logger.LogDebug(
            "Checking feature before creating policy. Feature name: {FeatureName}",
            featureName
        );

        bool featureExists = await FeatureExistsInDatabaseAsync(featureName);

        if (!featureExists)
        {
            _logger.LogError(
                "Feature does not exist in database. Feature name: {FeatureName}",
                featureName
            );

            return null;
        }

        _logger.LogDebug(
            "Creating authorization policy for feature. Feature name: {FeatureName}",
            featureName
        );

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new FeatureRequirement(featureName))
            .Build();
    }

    /// <summary>
    /// Checks whether a feature with the specified name exists in the database.
    /// </summary>
    /// <param name="featureName">The name of the feature to check.</param>
    /// <returns><c>true</c> if the feature exists; otherwise, <c>false</c>.</returns>
    private async Task<bool> FeatureExistsInDatabaseAsync(string featureName)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();

        LearningAssessmentDbContext dbContext = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();

        bool exists = await dbContext.Feature.AnyAsync(f => f.Name == featureName);

        _logger.LogDebug(
            "Feature existence check completed. Feature name: {FeatureName}, Exists: {Exists}",
            featureName,
            exists
        );

        return exists;
    }

    /// <summary>
    /// Invalidates the cached policy for the specified name, or clears the entire policy cache if no name is provided.
    /// </summary>
    /// <param name="policyName">
    /// The specific policy name to invalidate, or <c>null</c> to clear all cached policies.
    /// </param>
    public static void InvalidateCache(string? policyName = null)
    {
        if (!string.IsNullOrWhiteSpace(policyName))
        {
            _policyCache.TryRemove(policyName, out _);
        }
        else
        {
            _policyCache.Clear();
        }
    }
}
