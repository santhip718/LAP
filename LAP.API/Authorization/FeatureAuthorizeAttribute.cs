using LAP.Application.Constant;
using Microsoft.AspNetCore.Authorization;

namespace LAP.API.Authorization;

/// <summary>
/// Custom authorization attribute that enforces feature-based access by generating a policy name from the specified feature.
/// </summary>
public class FeatureAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureAuthorizeAttribute"/> class.
    /// </summary>
    /// <param name="featureName">The name of the feature required for access.</param>
    public FeatureAuthorizeAttribute(string featureName)
    {
        Policy = $"{CommonConstants.PolicyPrefix}{featureName}";
    }
}
