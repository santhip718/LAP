using System.Linq.Expressions;
using LAP.Domain.Entity;

namespace LAP.Application.Interface.IRepository;

/// <summary>
/// Provides a base repository with common data access methods.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IBaseRepository<T>
    where T : class
{
    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    void Create(T entity);

    /// <summary>
    /// Creates a new entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a range of entities in the repository asynchronously.
    /// </summary>
    /// <param name="entityList">The list of entities to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateRangeAsync(List<T> entityList, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a range of new entities in the repository.
    /// </summary>
    /// <param name="entityList">The list of entities to create.</param>
    void CreateRange(List<T> entityList);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity by its identifier using ExecuteDeleteAsync and returns the number of affected rows.
    /// </summary>
    /// <param name="entity">The entity to delete (only the Id is used).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows affected (0 if the entity was not found).</returns>
    Task<int> ExecuteDeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a range of existing entities.
    /// </summary>
    /// <param name="entityList">The list of entities to delete.</param>
    void DeleteRange(List<T> entityList);

    /// <summary>
    /// Finds entities in the repository that match the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of matching entities.</returns>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Finds entities in the repository that match the specified condition with tracking.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of matching entities.</returns>
    IQueryable<T> FindByConditionWithTracking(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Finds the first entity in the repository that matches the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <returns>The first matching entity, or null if no match is found.</returns>
    T? FindFirstByCondition(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <returns>The number of rows affected.</returns>
    int SaveChanges();

    /// <summary>
    /// Determines whether any entities in the repository match the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <returns><c>true</c> if any entities match the condition; otherwise, <c>false</c>.</returns>
    bool AnyByCondition(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Asynchronously determines whether any entities in the repository match the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if any entities match the condition; otherwise, <c>false</c>.</returns>
    Task<bool> AnyByConditionAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously finds the first entity in the repository that matches the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first matching entity, or null if no match is found.</returns>
    Task<T?> FindFirstByConditionAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously finds the first entity in the repository that matches the specified condition without tracking.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first matching entity, or null if no match is found.</returns>
    Task<T?> FindFirstByConditionAsNoTrackingAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a bulk delete on entities that match the specified condition.
    /// </summary>
    /// <param name="expression">The condition to match for deletion.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteDeleteAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a single active entity matching the condition without EF Core tracking.
    /// </summary>
    /// <param name="condition">The expression to filter entities.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the matching entity if found; otherwise, null.</returns>
    Task<T?> GetSingleByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active entities.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves entities by condition without tracking.
    /// </summary>
    IQueryable<T> GetByConditionNoTracking(Expression<Func<T, bool>> condition);

    #region Create Methods

    /// <summary>
    /// Creates a new entity and returns it along with a success message.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created entity and a success message.</returns>
    Task<(T Entity, string Message)> CreateWithResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a new entity and returns only the entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created entity.</returns>
    Task<T> CreateWithoutResponseAsync(T entity, CancellationToken cancellationToken = default);

    #endregion

    #region Post Methods

    /// <summary>
    /// Posts a new entity and returns it along with a success message.
    /// </summary>
    /// <param name="entity">The entity to post.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the posted entity and a success message.</returns>
    Task<(T Entity, string Message)> PostWithResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Posts a new entity and returns only the entity.
    /// </summary>
    /// <param name="entity">The entity to post.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the posted entity.</returns>
    Task<T> PostWithoutResponseAsync(T entity, CancellationToken cancellationToken = default);

    #endregion

    #region Helper Methods

    /// <summary>
    /// Counts all active records with tracking enabled.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of all active records.</returns>
    Task<int> CountAllTrackingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts active records matching the condition with tracking enabled.
    /// </summary>
    /// <param name="condition">The expression to filter records.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of matching active records.</returns>
    Task<int> CountByConditionTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Counts all active records without tracking.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of all active records.</returns>
    Task<int> CountAllNoTrackingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts active records matching the condition without tracking.
    /// </summary>
    /// <param name="condition">The expression to filter records.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of matching active records.</returns>
    Task<int> CountByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if any active entity matches the condition with tracking enabled.
    /// </summary>
    /// <param name="condition">The expression to check for matches.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any matching entity exists; otherwise, false.</returns>
    Task<bool> AnyByConditionTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if any active entity matches the condition without tracking.
    /// </summary>
    /// <param name="condition">The expression to check for matches.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any matching entity exists; otherwise, false.</returns>
    Task<bool> AnyByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Batch Operations

    /// <summary>
    /// Adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of added entities.</returns>
    Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a range of entities in the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of updated entities.</returns>
    Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Soft deletes a range of entities by marking them as inactive.
    /// </summary>
    /// <param name="entities">The collection of entities to soft delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SoftDeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Soft deletes entities matching the condition by setting IsActive to false.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> SoftDeleteAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Hard deletes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to hard delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HardDeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Transaction & Unit of Work

    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

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

    #endregion
}
