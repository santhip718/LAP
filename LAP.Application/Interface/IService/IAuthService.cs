using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for authentication-related operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email already exists.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new person entity and saves to generate the Id.
    /// </summary>
    Task AddPersonAsync(Person person, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user entity and saves to generate the Id.
    /// </summary>
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user secret entity.
    /// </summary>
    Task AddUserSecretAsync(UserSecret userSecret, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user-role mapping entity.
    /// </summary>
    Task AddUserRoleMappingAsync(
        UserRoleMapping userRoleMapping,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Saves pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the name of a role by its unique identifier.
    /// </summary>
    Task<string?> GetRoleNameByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token by its value.
    /// </summary>
    Task<RefreshToken?> GetRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new refresh token to the database.
    /// </summary>
    Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Marks a refresh token as revoked.
    /// </summary>
    Task RevokeRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    );
}
