using LAP.Application.Interface;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="IAuthService"/> using <see cref="IRepositoryWrapper"/>.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public AuthService(IRepositoryWrapper repositoryWrapper, ICustomLogger<AuthService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a user by email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The user if found; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user by email {Email}.", email);
        return await _repositoryWrapper
            .User.FindByCondition(u => u.IsActive && u.Person.Email == email)
            .Include(u => u.Person)
            .Include(u => u.UserSecret)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Checks whether the specified email address already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns><c>true</c> if the email exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking existence of email {Email}.", email);
        return await _repositoryWrapper
            .Repository<Person>()
            .AnyByConditionAsync(p => p.IsActive && p.Email == email, cancellationToken);
    }

    /// <summary>
    /// Adds a new person record.
    /// </summary>
    /// <param name="person">The person entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddPersonAsync(Person person, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding person {PersonId}.", person.Id);
        await _repositoryWrapper.Repository<Person>().CreateAsync(person, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new user record.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding user {UserId}.", user.Id);
        await _repositoryWrapper.User.CreateAsync(user, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a user secret record.
    /// </summary>
    /// <param name="userSecret">The user secret entity.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddUserSecretAsync(
        UserSecret userSecret,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding user secret for user {UserId}.", userSecret.UserId);
        await _repositoryWrapper
            .Repository<UserSecret>()
            .CreateAsync(userSecret, cancellationToken);
    }

    /// <summary>
    /// Adds a user-role mapping record.
    /// </summary>
    /// <param name="userRoleMapping">The user-role mapping entity.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddUserRoleMappingAsync(
        UserRoleMapping userRoleMapping,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding user-role mapping for user {UserId}.", userRoleMapping.UserId);
        await _repositoryWrapper
            .Repository<UserRoleMapping>()
            .CreateAsync(userRoleMapping, cancellationToken);
    }

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving authentication changes.");
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the role name associated with the specified role identifier.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The role name if found; otherwise, <c>null</c>.</returns>
    public async Task<string?> GetRoleNameByIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving role name for role {RoleId}.", roleId);
        return await _repositoryWrapper
            .Repository<RefTerm>()
            .FindByCondition(r => r.IsActive && r.Id == roleId)
            .Select(r => r.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a refresh token entity by its token value.
    /// </summary>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The refresh token entity if found; otherwise, <c>null</c>.</returns>
    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        return await _repositoryWrapper
            .RefreshToken.FindByCondition(x => x.IsActive && x.Token == refreshToken)
            .Include(x => x.User)
                .ThenInclude(x => x.Person)
            .Include(x => x.User)
                .ThenInclude(x => x.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a refresh token record.
    /// </summary>
    /// <param name="refreshToken">The refresh token entity.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding refresh token for user {UserId}.", refreshToken.UserId);
        await _repositoryWrapper.RefreshToken.CreateAsync(refreshToken, cancellationToken);
    }

    /// <summary>
    /// Revokes the specified refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token entity to revoke.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    public async Task RevokeRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Revoking refresh token for user {UserId}.", refreshToken.UserId);
        refreshToken.IsRevoked = true;
        _repositoryWrapper.RefreshToken.Update(refreshToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }
}
