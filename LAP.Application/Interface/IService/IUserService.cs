using LAP.Domain.Entity;

namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides data-access abstraction for user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all users with their full details.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of users with details.</returns>
    Task<List<User>> GetAllUserWithDetailAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their ID with the Person navigation property loaded.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user with Person loaded if found; otherwise, null.</returns>
    Task<User?> GetUserByIdWithPersonAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their ID with full details.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdWithDetailAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their ID including secret information.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdWithSecretAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their ID including enrollment information.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdWithEnrollmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    void UpdateUser(User user);

    /// <summary>
    /// Deletes a user by its identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
