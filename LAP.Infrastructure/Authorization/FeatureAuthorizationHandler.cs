using System.Security.Claims;
using LAP.Application.Authorization;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Microsoft.AspNetCore.Authorization;

namespace LAP.Infrastructure.Authorization;

/// <summary>
/// Handles authorization requirements by checking if the user's roles have the required feature permission.
/// </summary>
public class FeatureAuthorizationHandler : AuthorizationHandler<FeatureRequirement>
{
    private readonly IPermissionCacheService _permissionCacheService;
    private readonly ICustomLogger<FeatureAuthorizationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureAuthorizationHandler"/> class.
    /// </summary>
    /// <param name="permissionCacheService">The permission cache service for retrieving role-based permissions.</param>
    /// <param name="logger">The custom logger instance.</param>
    public FeatureAuthorizationHandler(
        IPermissionCacheService permissionCacheService,
        ICustomLogger<FeatureAuthorizationHandler> logger
    )
    {
        _permissionCacheService = permissionCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Evaluates whether the current user has the required feature by checking permissions for each of their roles.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The feature requirement to evaluate.</param>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        FeatureRequirement requirement
    )
    {
        _logger.LogDebug(
            "Evaluating feature authorization. FeatureName: {FeatureName}",
            requirement.FeatureName
        );

        List<string> roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (!roles.Any())
        {
            _logger.LogError(
                "Authorization failed. No roles found for current user. FeatureName: {FeatureName}",
                requirement.FeatureName
            );

            return;
        }

        _logger.LogDebug("User roles found. Roles: {Roles}", string.Join(", ", roles));

        foreach (string role in roles)
        {
            _logger.LogDebug(
                "Checking permissions for Role: {Role}, FeatureName: {FeatureName}",
                role,
                requirement.FeatureName
            );

            HashSet<string> permissions = await _permissionCacheService.GetPermissionsAsync(role);

            if (permissions.Contains(requirement.FeatureName))
            {
                _logger.LogDebug(
                    "Authorization succeeded. Role: {Role}, FeatureName: {FeatureName}",
                    role,
                    requirement.FeatureName
                );

                context.Succeed(requirement);
                return;
            }
        }

        _logger.LogError(
            "Authorization failed. No matching permission found. FeatureName: {FeatureName}",
            requirement.FeatureName
        );
    }
}
