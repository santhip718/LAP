using Microsoft.AspNetCore.Authorization;

namespace LAP.Application.Authorization;

/// <summary>
/// Represents an authorization requirement that a specific feature must be granted to the user.
/// </summary>
public class FeatureRequirement : IAuthorizationRequirement
{
    /// <summary>Gets the name of the required feature.</summary>
    public string FeatureName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureRequirement"/> class.
    /// </summary>
    /// <param name="featureName">The name of the feature required for access.</param>
    public FeatureRequirement(string featureName)
    {
        FeatureName = featureName;
    }
}
