using AutoMapper;
using FluentValidation;
using LAP.Application.Behaviors;
using LAP.Application.Feature.Auth.Command;
using LAP.Application.Helpers;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Application.Mapping;
using LAP.Application.Service;
using LAP.Infrastructure.Helper;
using LAP.Infrastructure.Logging;
using LAP.Infrastructure.Services;
using MediatR;

namespace LAP.API.Extensions;

/// <summary>
/// Provides extension methods for registering application-specific services in the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services, helpers, MediatR, AutoMapper, and FluentValidation in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add application services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        // Logger
        services.AddSingleton(typeof(ICustomLogger<>), typeof(CustomLogger<>));

        // Helpers
        services.AddScoped<IJwtHelper, JwtHelper>();
        services.AddScoped<IQuestionParser, ExcelQuestionParser>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAssessmentService, AssessmentService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseContentService, CourseContentService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IReferenceCacheService, ReferenceCacheService>();
        services.AddScoped<IPermissionCacheService, PermissionCacheService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IForumService, ForumService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        // Request Context
        services.AddScoped<IRequestContext, RequestContext>();

        // AutoMapper
        services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly)
        );

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<RegisterCommand>();

        // Pipeline behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
