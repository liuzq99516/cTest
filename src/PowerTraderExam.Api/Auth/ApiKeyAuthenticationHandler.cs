using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Api.Auth;

public static class AuthSchemes
{
    public const string ApiKey = "ApiKey";
}

public static class AuthRoles
{
    public const string Admin = "Admin";
    public const string ExamServer = "ExamServer";
}

public static class AuthClaims
{
    public const string ServerId = "server_id";
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApiKeyOptions _apiKeyOptions;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IOptions<ApiKeyOptions> apiKeyOptions)
        : base(options, logger, encoder, clock)
    {
        _apiKeyOptions = apiKeyOptions.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKeyValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var apiKey = apiKeyValues.ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!string.IsNullOrEmpty(_apiKeyOptions.Admin) && apiKey == _apiKeyOptions.Admin)
        {
            var adminIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, AuthRoles.Admin)
            }, AuthSchemes.ApiKey);

            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(adminIdentity), AuthSchemes.ApiKey)));
        }

        foreach (var (serverId, key) in _apiKeyOptions.Servers)
        {
            if (apiKey == key)
            {
                var serverIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, serverId),
                    new Claim(ClaimTypes.Role, AuthRoles.ExamServer),
                    new Claim(AuthClaims.ServerId, serverId)
                }, AuthSchemes.ApiKey);

                return Task.FromResult(AuthenticateResult.Success(
                    new AuthenticationTicket(new ClaimsPrincipal(serverIdentity), AuthSchemes.ApiKey)));
            }
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
    }
}
