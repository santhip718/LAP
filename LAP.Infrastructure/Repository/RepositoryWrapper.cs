using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Infrastructure.Repository;

/// <summary>
/// Implementation of <see cref="IRepositoryWrapper"/> that provides a single entry point
/// for all repositories in the system.
/// </summary>
public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly LearningAssessmentDbContext _dbContext;
    private readonly ICustomLogger<RepositoryWrapper> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories = new();

    private IUserRepository? _user;
    private IRefreshTokenRepository? _refreshToken;
    private IAssessmentRepository? _assessment;
    private ILeaderboardRepository? _leaderboard;
    private IReviewRepository? _review;
    private ICourseRepository? _course;
    private ICourseContentRepository? _courseContent;
    private IForumRepository? _forum;
    private IEnrollmentRepository? _enrollment;
    private IUserCourseProgressRepository? _userCourseProgress;
    private IRoleFeatureMappingRepository? _roleFeatureMapping;
    private IAssessmentHistoryRepository? _assessmentHistory;
    private IAssessmentAnswerRepository? _assessmentAnswer;
    private ITierRepository? _tier;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryWrapper"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="serviceProvider">The service provider for resolving loggers.</param>
    public RepositoryWrapper(
        LearningAssessmentDbContext dbContext,
        ICustomLogger<RepositoryWrapper> logger,
        IServiceProvider serviceProvider
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the repository for user operations.
    /// </summary>
    public IUserRepository User =>
        _user ??= new UserRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<User>>>()
        );

    /// <summary>
    /// Gets the repository for managing refresh tokens.
    /// </summary>
    public IRefreshTokenRepository RefreshToken =>
        _refreshToken ??= new RefreshTokenRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<RefreshToken>>>()
        );

    /// <summary>
    /// Gets the repository for managing course reviews.
    /// </summary>
    public IReviewRepository Review =>
        _review ??= new ReviewRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<Review>>>()
        );

    /// <summary>
    /// Gets the repository for managing courses.
    /// </summary>
    public ICourseRepository Course =>
        _course ??= new CourseRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<Course>>>()
        );

    /// <summary>
    /// Gets the repository for managing course contents.
    /// </summary>
    public ICourseContentRepository CourseContent =>
        _courseContent ??= new CourseContentRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<CourseContent>>>()
        );

    /// <summary>
    /// Gets the repository for forum message operations.
    /// </summary>
    public IForumRepository Forum =>
        _forum ??= new ForumRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<ForumMessage>>>()
        );

    /// <summary>
    /// Gets the repository for managing enrollments.
    /// </summary>
    public IEnrollmentRepository Enrollment =>
        _enrollment ??= new EnrollmentRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<Enrollment>>>()
        );

    /// <summary>
    /// Gets the repository for managing user course progress.
    /// </summary>
    public IUserCourseProgressRepository UserCourseProgress =>
        _userCourseProgress ??= new UserCourseProgressRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<UserCourseProgress>>>()
        );

    /// <summary>
    /// Gets the repository for managing assessments.
    /// </summary>
    public IAssessmentRepository Assessment =>
        _assessment ??= new AssessmentRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<Assessment>>>()
        );

    /// <summary>
    /// Gets the repository for managing role-feature mappings.
    /// </summary>
    public IRoleFeatureMappingRepository RoleFeatureMapping =>
        _roleFeatureMapping ??= new RoleFeatureMappingRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<RoleFeatureMapping>>>()
        );

    /// <summary>
    /// Gets the repository for managing assessment history records.
    /// </summary>
    public IAssessmentHistoryRepository AssessmentHistory =>
        _assessmentHistory ??= new AssessmentHistoryRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<AssessmentHistory>>>()
        );

    /// <summary>
    /// Gets the repository for managing assessment answers.
    /// </summary>
    public IAssessmentAnswerRepository AssessmentAnswer =>
        _assessmentAnswer ??= new AssessmentAnswerRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<AssessmentAnswer>>>()
        );

    /// <summary>
    /// Gets the repository for managing tier reference data.
    /// </summary>
    public ITierRepository Tier =>
        _tier ??= new TierRepository(
            _dbContext,
            _serviceProvider.GetRequiredService<ICustomLogger<BaseRepository<RefTerm>>>()
        );

    /// <summary>
    /// Gets the repository for leaderboard queries.
    /// </summary>
    public ILeaderboardRepository Leaderboard =>
        _leaderboard ??= _serviceProvider.GetRequiredService<ILeaderboardRepository>();

    /// <summary>
    /// Gets a generic repository for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The type of the domain entity.</typeparam>
    /// <returns>A generic repository for the specified entity type.</returns>
    public IBaseRepository<T> Repository<T>()
        where T : BaseEntity
    {
        Type type = typeof(T);

        if (_repositories.TryGetValue(type, out object? repo))
            return (IBaseRepository<T>)repo;

        IBaseRepository<T> instance = _serviceProvider.GetRequiredService<IBaseRepository<T>>();
        _repositories[type] = instance;

        return instance;
    }

    /// <summary>
    /// Asynchronously saves all changes made in the current unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes to the database.");
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Executes an asynchronous operation within a database transaction.
    /// </summary>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Beginning transaction");
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operation();
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transaction committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous operation that returns a result within a database transaction.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Beginning transaction");
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            TResult result = await operation();
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transaction committed");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
