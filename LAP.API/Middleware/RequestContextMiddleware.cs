using System.Security.Claims;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;

namespace LAP.API.Middleware;

/// <summary>
/// Middleware responsible for populating the current request context
/// with authenticated user information extracted from JWT claims.
///
/// This middleware executes after authentication and makes user-specific
/// data such as UserId, Email, Role, and authentication status available
/// throughout the application via <see cref="IRequestContext"/>.
/// </summary>
public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICustomLogger<RequestContextMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestContextMiddleware"/> class.
    /// </summary>
    /// <param name="next">
    /// The next middleware delegate in the HTTP request pipeline.
    /// </param>
    /// <param name="logger">
    /// The logger instance for logging request context operations.
    /// </param>
    public RequestContextMiddleware(
        RequestDelegate next,
        ICustomLogger<RequestContextMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Extracts authenticated user information from the current
    /// <see cref="ClaimsPrincipal"/> and stores it in the request context
    /// for downstream services and components.
    /// </summary>
    /// <param name="context">
    /// The current HTTP context.
    /// </param>
    /// <param name="requestContext">
    /// The request-scoped context used to store authenticated user details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous middleware operation.
    /// </returns>
    public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            requestContext.IsAuthenticated = true;

            Guid? userId = Guid.TryParse(
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var id
            )
                ? id
                : null;

            requestContext.UserId = userId;

            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "Anonymous";
            requestContext.Email = email;

            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "None";
            requestContext.Role = role;

            _logger.LogDebug(
                "Context populated for authenticated user {UserId} ({Email}) with role {Role}",
                userId,
                email,
                role
            );
        }
        else
        {
            _logger.LogDebug("Request is anonymous, context not populated");
        }

        await _next(context);
    }
}
