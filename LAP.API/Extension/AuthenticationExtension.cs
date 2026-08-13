using System.Text;
using LAP.API.Authorization;
using LAP.Application.DTO;
using LAP.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace LAP.API.Extensions;

/// <summary>
/// Provides extension methods for configuring authentication and authorization in the application.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Configures JWT-based authentication for the application.
    /// </summary>
    /// <param name="services">The service collection to add authentication to.</param>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        services.Configure<JwtSettings>(jwtSettings);

        var secretKey = jwtSettings["SecretKey"] ?? string.Empty;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],

                    ValidAudience = jwtSettings["Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };
            });

        return services;
    }

    /// <summary>
    /// Registers authorization policies and requirements in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add authorization policies to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();

        services.AddScoped<IAuthorizationHandler, FeatureAuthorizationHandler>();

        return services;
    }
}
