using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace LAP.API.Extensions;

/// <summary>
/// Provides extension methods for configuring Swagger/OpenAPI documentation in the service collection.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configures Swagger generator and security definitions for API documentation.
    /// </summary>
    /// <param name="services">The service collection to add Swagger documentation to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.SwaggerDoc(
                "v1",
                new OpenApiInfo { Title = "Learning Assessment Portal API", Version = "v1" }
            );

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Token like this: Bearer {your token}",
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        return services;
    }
}
