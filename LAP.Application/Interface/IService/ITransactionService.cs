namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides a dedicated service for managing database transactions and unit-of-work persistence.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation that returns a result within a database transaction.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes an asynchronous operation within a database transaction.
    /// </summary>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    );
}
