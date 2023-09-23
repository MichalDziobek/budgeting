using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApi.Tests.Integration.Common;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    private const string UserId = "UserId";

    private readonly ICurrentUserService _currentUserService;

    public TestAuthHandler(
        ICurrentUserService currentUserService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _currentUserService = currentUserService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test user"),
            // Extract User ID from the request headers if it exists,
            // otherwise use the default User ID from the options.
            Context.Request.Headers.TryGetValue(UserId, out var userId)
                ? new Claim(ClaimTypes.NameIdentifier, userId[0] ?? string.Empty)
                : new Claim(ClaimTypes.NameIdentifier, _currentUserService.UserId ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}