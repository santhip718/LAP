using LAP.Application.DTO.Auth;

namespace LAP.Application.Interface.IHelper;

/// <summary>
/// Defines methods for generating JWT authentication tokens with user claims.
/// </summary>
public interface IJwtHelper
{
    /// <summary>
    /// Generates an access token and refresh token for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="roles">The list of roles assigned to the user.</param>
    /// <returns>An <see cref="AuthTokenResponseDto"/> containing the generated tokens.</returns>
    AuthTokenResponseDto GenerateToken(
        Guid userId,
        string email,
        string fullName,
        List<string> roles
    );
    string GenerateRefreshToken();
}
