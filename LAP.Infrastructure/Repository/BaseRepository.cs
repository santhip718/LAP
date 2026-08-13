using System.Linq.Expressions;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Provides a base implementation for common data repository operations using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of the domain entity, which must inherit from <see cref="BaseEntity"/>.</typeparam>
public class BaseRepository<T> : IBaseRepository<T>
    where T : BaseEntity
{
    private const string InMemoryProviderName = "Microsoft.EntityFrameworkCore.InMemory";

    /// <summary>
    /// The database context.
    /// </summary>
    protected readonly LearningAssessmentDbContext _dbContext;

    /// <summary>
    /// The database set for the entity.
    /// </summary>
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// The custom logger for structured logging.
    /// </summary>
    protected readonly ICustomLogger<BaseRepository<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to use.</param>
    /// <param name="logger">The logger instance for this repository.</param>
    public BaseRepository(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<BaseRepository<T>> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _dbSet = _dbContext.Set<T>();
    }

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    public void Create(T entity)
    {
        _logger.LogDebug("Creating {Entity}", typeof(T).Name);
        _dbSet.Add(entity);
    }

    /// <summary>
    /// Creates a new entity in the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating {Entity} asynchronously", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity and returns it.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The added entity.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding new {Entity}", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Creates multiple entities in the database asynchronously.
    /// </summary>
    /// <param name="entityList">The list of entities to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task CreateRangeAsync(
        List<T> entityList,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Creating a range of {Entity} asynchronously", typeof(T).Name);
        await _dbSet.AddRangeAsync(entityList, cancellationToken);
    }

    /// <summary>
    /// Creates multiple entities in the database.
    /// </summary>
    /// <param name="entityList">The list of entities to create.</param>
    public void CreateRange(List<T> entityList)
    {
        _logger.LogDebug("Creating a range of {Entity}", typeof(T).Name);
        _dbSet.AddRange(entityList);
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity)
    {
        _logger.LogDebug("Updating {Entity}", typeof(T).Name);
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Deletes an entity by its id using ExecuteDeleteAsync.
    /// Falls back to tracked removal for in-memory provider.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> ExecuteDeleteAsync(
        T entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Deleting {Entity} with id {Id} using ExecuteDeleteAsync",
            typeof(T).Name,
            entity.Id
        );

        if (_dbContext.Database.ProviderName == InMemoryProviderName)
        {
            var trackedEntity = await _dbSet.FindAsync(
                new object[] { entity.Id },
                cancellationToken
            );
            if (trackedEntity != null)
            {
                _dbSet.Remove(trackedEntity);
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return 0;
        }

        return await _dbSet.Where(e => e.Id == entity.Id).ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a range of entities from the database.
    /// </summary>
    /// <param name="entityList">The list of entities to delete.</param>
    public void DeleteRange(List<T> entityList)
    {
        _logger.LogDebug("Deleting a range of {Entity}", typeof(T).Name);
        _dbSet.RemoveRange(entityList);
    }

    /// <summary>
    /// Finds entities matching the given expression with no-tracking enabled.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>A queryable of matching entities.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        _logger.LogDebug("Finding {Entity} by condition (AsNoTracking)", typeof(T).Name);
        return _dbSet.Where(expression).AsNoTracking();
    }

    /// <summary>
    /// Finds entities matching the given expression with tracking enabled.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>A queryable of matching entities.</returns>
    public IQueryable<T> FindByConditionWithTracking(Expression<Func<T, bool>> expression)
    {
        _logger.LogDebug("Finding {Entity} by condition (Tracking)", typeof(T).Name);
        return _dbSet.Where(expression);
    }

    /// <summary>
    /// Finds the first entity matching the given expression.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>The matching entity or null.</returns>
    public T? FindFirstByCondition(Expression<Func<T, bool>> expression)
    {
        _logger.LogDebug("Finding first {Entity} by condition", typeof(T).Name);
        return _dbSet.Where(expression).FirstOrDefault();
    }

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <returns>The number of state entries written.</returns>
    public int SaveChanges()
    {
        _logger.LogDebug("Saving changes");
        return _dbContext.SaveChanges();
    }

    /// <summary>
    /// Checks if any entity matches the given expression.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>True if any matching entity exists.</returns>
    public bool AnyByCondition(Expression<Func<T, bool>> expression)
    {
        _logger.LogDebug("Checking any {Entity} by condition", typeof(T).Name);
        return _dbSet.Any(expression);
    }

    /// <summary>
    /// Checks if any entity matches the given expression asynchronously.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if any matching entity exists.</returns>
    public async Task<bool> AnyByConditionAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking any {Entity} by condition asynchronously", typeof(T).Name);
        return await _dbSet.AnyAsync(expression, cancellationToken);
    }

    /// <summary>
    /// Finds the first entity matching the given expression asynchronously.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity or null.</returns>
    public async Task<T?> FindFirstByConditionAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Finding first {Entity} by condition asynchronously", typeof(T).Name);
        return await _dbSet.Where(expression).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Finds the first entity matching the given expression asynchronously with no-tracking enabled.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity or null.</returns>
    public async Task<T?> FindFirstByConditionAsNoTrackingAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Finding first {Entity} by condition (AsNoTracking) asynchronously",
            typeof(T).Name
        );
        return await _dbSet.Where(expression).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a bulk delete based on the given expression.
    /// Falls back to tracked removal for in-memory provider.
    /// </summary>
    /// <param name="expression">The filter expression to match entities to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows deleted.</returns>
    public async Task<int> ExecuteDeleteAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Executing bulk delete for {Entity}", typeof(T).Name);

        if (_dbContext.Database.ProviderName == InMemoryProviderName)
        {
            var entities = await _dbSet.Where(expression).ToListAsync(cancellationToken);
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return 0;
        }

        return await _dbSet.Where(expression).ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a single active entity matching the given condition with no-tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity or null.</returns>
    public async Task<T?> GetSingleByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Fetching single {Entity} by condition (No-Tracking)", typeof(T).Name);
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.IsActive)
            .SingleOrDefaultAsync(condition, cancellationToken);
    }

    /// <summary>
    /// Retrieves an active entity by its unique identifier.
    /// </summary>
    /// <param name="id">The entity id.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity or null.</returns>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching {Entity} by id {Id}", typeof(T).Name, id);
        return await _dbSet.FirstOrDefaultAsync(
            e => e.IsActive && e.Id.Equals(id),
            cancellationToken
        );
    }

    /// <summary>
    /// Retrieves all active entities.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of all active entities.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all {Entity}", typeof(T).Name);
        return await _dbSet.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a queryable of active entities matching the given condition with no-tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <returns>A queryable of matching active entities.</returns>
    public IQueryable<T> GetByConditionNoTracking(Expression<Func<T, bool>> condition)
    {
        _logger.LogDebug(
            "Fetching {Entity} by condition (No-Tracking) - Queryable",
            typeof(T).Name
        );
        return _dbSet.AsNoTracking().Where(e => e.IsActive).Where(condition);
    }

    #region Create Methods

    /// <summary>
    /// Creates a new entity and returns it along with a success message.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A tuple containing the created entity and a success message.</returns>
    public async Task<(T Entity, string Message)> CreateWithResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Creating {Entity} (With Response)", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return (entity, $"{typeof(T).Name} created successfully.");
    }

    /// <summary>
    /// Creates a new entity and returns it without a response message.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created entity.</returns>
    public async Task<T> CreateWithoutResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Creating {Entity} (Without Response)", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    #endregion

    #region Post Methods

    /// <summary>
    /// Posts a new entity and returns it along with a success message.
    /// </summary>
    /// <param name="entity">The entity to post.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A tuple containing the posted entity and a success message.</returns>
    public async Task<(T Entity, string Message)> PostWithResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Posting {Entity} (With Response)", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return (entity, $"{typeof(T).Name} created successfully.");
    }

    /// <summary>
    /// Posts a new entity and returns it without a response message.
    /// </summary>
    /// <param name="entity">The entity to post.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The posted entity.</returns>
    public async Task<T> PostWithoutResponseAsync(
        T entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Posting {Entity} (Without Response)", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Counts all active entities with tracking enabled.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The total count of active entities.</returns>
    public async Task<int> CountAllTrackingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Counting all {Entity} (Tracking)", typeof(T).Name);
        return await _dbSet.AsTracking().Where(e => e.IsActive).CountAsync(cancellationToken);
    }

    /// <summary>
    /// Counts active entities matching the given condition with tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The count of matching active entities.</returns>
    public async Task<int> CountByConditionTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Counting {Entity} by condition (Tracking)", typeof(T).Name);
        return await _dbSet
            .AsTracking()
            .Where(e => e.IsActive)
            .Where(condition)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Counts all active entities with no-tracking enabled.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The total count of active entities.</returns>
    public async Task<int> CountAllNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Counting all {Entity} (No-Tracking)", typeof(T).Name);
        return await _dbSet.AsNoTracking().Where(e => e.IsActive).CountAsync(cancellationToken);
    }

    /// <summary>
    /// Counts active entities matching the given condition with no-tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The count of matching active entities.</returns>
    public async Task<int> CountByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Counting {Entity} by condition (No-Tracking)", typeof(T).Name);
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.IsActive)
            .Where(condition)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if any active entity matches the given condition with tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if a matching active entity exists.</returns>
    public async Task<bool> AnyByConditionTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking any {Entity} by condition (Tracking)", typeof(T).Name);
        return await _dbSet
            .AsTracking()
            .Where(e => e.IsActive)
            .AnyAsync(condition, cancellationToken);
    }

    /// <summary>
    /// Checks if any active entity matches the given condition with no-tracking enabled.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if a matching active entity exists.</returns>
    public async Task<bool> AnyByConditionNoTrackingAsync(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Checking any {Entity} by condition (No-Tracking)", typeof(T).Name);
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.IsActive)
            .AnyAsync(condition, cancellationToken);
    }

    #endregion

    #region Batch Operations

    /// <summary>
    /// Adds a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The added entities.</returns>
    public async Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Adding a range of {Entity} asynchronously", typeof(T).Name);
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    /// <summary>
    /// Updates a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated entities.</returns>
    public async Task<IEnumerable<T>> UpdateRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Updating a range of {Entity} asynchronously", typeof(T).Name);
        _dbSet.UpdateRange(entities);
        return await Task.FromResult(entities);
    }

    /// <summary>
    /// Soft deletes a range of entities by setting IsActive to false.
    /// </summary>
    /// <param name="entities">The entities to soft delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task SoftDeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Soft deleting a range of {Entity} asynchronously", typeof(T).Name);
        foreach (var entity in entities)
        {
            entity.IsActive = false;
        }
        _dbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Soft deletes entities matching the condition by setting IsActive to false.
    /// </summary>
    /// <param name="expression">The condition to match.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> SoftDeleteAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Soft deleting {Entity} matching condition", typeof(T).Name);

        if (_dbContext.Database.ProviderName == InMemoryProviderName)
        {
            List<T> entities = await _dbSet.Where(expression).ToListAsync(cancellationToken);
            foreach (T entity in entities)
            {
                entity.IsActive = false;
            }
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return await _dbSet
            .Where(expression)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(e => e.IsActive, false),
                cancellationToken
            );
    }

    /// <summary>
    /// Hard deletes a range of entities from the database.
    /// </summary>
    /// <param name="entities">The entities to hard delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task HardDeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Hard deleting a range of {Entity} asynchronously", typeof(T).Name);
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    #endregion

    #region Transaction & Unit of Work

    /// <summary>
    /// Saves all pending changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes asynchronously");
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Executes an operation within a database transaction.
    /// Rolls back the transaction if an exception occurs.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        try
        {
            _logger.LogDebug("Executing transaction.");
            await operation();
            await transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transaction committed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes an operation that returns a result within a database transaction.
    /// Rolls back the transaction if an exception occurs.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        try
        {
            _logger.LogDebug("Executing transaction with result.");
            TResult result = await operation();
            await transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transaction committed.");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion
}
