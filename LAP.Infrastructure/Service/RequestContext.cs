using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using Microsoft.AspNetCore.Http;

namespace LAP.Infrastructure.Services;

/// <summary>
/// Implements the request context by reading and writing user data from the HTTP context items collection.
/// </summary>
public class RequestContext : IRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomLogger<RequestContext> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestContext"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current request.</param>
    /// <param name="logger">The logger instance for logging request context operations.</param>
    public RequestContext(
        IHttpContextAccessor httpContextAccessor,
        ICustomLogger<RequestContext> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private HttpContext? Context => _httpContextAccessor.HttpContext;

    /// <summary>Gets or sets the current user's unique identifier from the HTTP context.</summary>
    public Guid? UserId
    {
        get
        {
            var value = Context?.Items[CommonConstants.CONTEXT_USER_ID]?.ToString();
            Guid? parsed = Guid.TryParse(value, out var id) ? id : null;
            return parsed;
        }
        set { Context!.Items[CommonConstants.CONTEXT_USER_ID] = value; }
    }

    /// <summary>Gets or sets the current user's email address from the HTTP context.</summary>
    public string? Email
    {
        get => Context?.Items[CommonConstants.CONTEXT_EMAIL]?.ToString();
        set { Context!.Items[CommonConstants.CONTEXT_EMAIL] = value; }
    }

    /// <summary>Gets or sets the current user's role name from the HTTP context.</summary>
    public string? Role
    {
        get => Context?.Items[CommonConstants.CONTEXT_ROLE]?.ToString();
        set { Context!.Items[CommonConstants.CONTEXT_ROLE] = value; }
    }

    /// <summary>Gets or sets a value indicating whether the current request is authenticated.</summary>
    public bool IsAuthenticated
    {
        get =>
            bool.TryParse(
                Context?.Items[CommonConstants.CONTEXT_IS_AUTHENTICATED]?.ToString(),
                out var value
            ) && value;
        set { Context!.Items[CommonConstants.CONTEXT_IS_AUTHENTICATED] = value; }
    }
}
