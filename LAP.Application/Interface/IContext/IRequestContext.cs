namespace LAP.Application.Interface.IContext;

/// <summary>
/// Defines the current HTTP request context properties for the authenticated user.
/// </summary>
public interface IRequestContext
{
    /// <summary>
    /// Gets or sets the current user's unique identifier.
    /// </summary>
    Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the current user's email address.
    /// </summary>
    string? Email { get; set; }

    /// <summary>
    /// Gets or sets the current user's role name.
    /// </summary>
    string? Role { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; set; }
}
