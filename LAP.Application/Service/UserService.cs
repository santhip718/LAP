using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="IUserService"/> using <see cref="IRepositoryWrapper"/>.
/// </summary>
public class UserService : IUserService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper.</param>
    /// <param name="logger">Custom application logger.</param>
    public UserService(IRepositoryWrapper repositoryWrapper, ICustomLogger<UserService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users with their full details including person and role information.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of all active users with their details.</returns>
    public async Task<List<User>> GetAllUserWithDetailAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving all users with details.");

        return await _repositoryWrapper
            .User.GetByConditionNoTracking(u => true)
            .Include(u => u.Person)
                .ThenInclude(p => p.Designation)
            .Include(u => u.Person)
                .ThenInclude(p => p.Gender)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.CurrentTier)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching user if found and active; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user {UserId}.", id);

        return await _repositoryWrapper.User.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier with the Person navigation property loaded.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching user with Person loaded if found and active; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByIdWithPersonAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user {UserId} with person.", id);

        return await _repositoryWrapper
            .User.FindByConditionWithTracking(u => u.IsActive && u.Id == id)
            .Include(u => u.Person)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier including person and role details.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching user with full details if found and active; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByIdWithDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user {UserId} with details.", id);

        return await _repositoryWrapper
            .User.GetByConditionNoTracking(u => u.Id == id)
            .Include(u => u.Person)
                .ThenInclude(p => p.Designation)
            .Include(u => u.Person)
                .ThenInclude(p => p.Gender)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.CurrentTier)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier including authentication secret information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching user with secret data if found and active; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByIdWithSecretAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user {UserId} with secret.", id);

        return await _repositoryWrapper
            .User.GetByConditionNoTracking(u => u.Id == id)
            .Include(u => u.Person)
            .Include(u => u.UserSecret)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier including enrollment records.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching user with enrollments if found and active; otherwise, <c>null</c>.</returns>
    public async Task<User?> GetUserByIdWithEnrollmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Retrieving user {UserId} with enrollments.", id);

        return await _repositoryWrapper
            .User.GetByConditionNoTracking(u => u.Id == id)
            .Include(u => u.Person)
                .ThenInclude(p => p.Designation)
            .Include(u => u.Person)
                .ThenInclude(p => p.Gender)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.CurrentTier)
            .Include(u => u.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Category)
            .Include(u => u.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.DifficultyLevel)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user entity containing the updated property values.</param>
    public void UpdateUser(User user)
    {
        _logger.LogDebug("Updating user {UserId}.", user.Id);

        _repositoryWrapper.User.Update(user);
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="user">The user entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting user {UserId}.", id);
        return await _repositoryWrapper.User.SoftDeleteAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }
}
