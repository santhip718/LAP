using LAP.Application.Interface.IRepository;
using LAP.Domain.Entity;
using LAP.Infrastructure.Repository;

namespace LAP.API.Extensions;

/// <summary>
/// Provides extension methods for registering repositories in the service collection.
/// </summary>
public static class RepositoryCollectionExtensions
{
    /// <summary>
    /// Registers the repository wrapper and its associated repositories in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add repositories to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

        return services;
    }
}
