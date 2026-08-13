using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;

namespace LAP.Application.Service;

/// <summary>
/// Implementation of <see cref="ITransactionService"/> using the base repository's transaction support.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICustomLogger<TransactionService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionService"/> class.
    /// </summary>
    /// <param name="repositoryWrapper">The repository wrapper providing access to all data repositories.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public TransactionService(
        IRepositoryWrapper repositoryWrapper,
        ICustomLogger<TransactionService> logger
    )
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Saves all pending changes in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes to the database.");
        return await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Executes an asynchronous operation that returns a result within a database transaction.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="operation">The asynchronous function to execute within the transaction scope.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Executing transaction with result.");
        return await _repositoryWrapper.ExecuteInTransactionAsync(operation, cancellationToken);
    }

    /// <summary>
    /// Executes an asynchronous operation with no return value within a database transaction.
    /// </summary>
    /// <param name="operation">The asynchronous function to execute within the transaction scope.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Executing transaction.");
        await _repositoryWrapper.ExecuteInTransactionAsync(operation, cancellationToken);
    }
}
