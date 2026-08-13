using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LAP.IntegrationTest;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthScheme = "TestScheme";
    public const string TestUserIdHeader = "TestUserId";
    public const string TestUserEmailHeader = "TestUserEmail";
    public const string TestUserRoleHeader = "TestRole";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(TestUserIdHeader, out var userIdValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = userIdValues.FirstOrDefault() ?? Guid.NewGuid().ToString();
        var email = Request.Headers.TryGetValue(TestUserEmailHeader, out var emailValues)
            ? emailValues.FirstOrDefault() ?? "test@example.com"
            : "test@example.com";
        var role = Request.Headers.TryGetValue(TestUserRoleHeader, out var roleValues)
            ? roleValues.FirstOrDefault() ?? "Admin"
            : "Admin";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, AuthScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
