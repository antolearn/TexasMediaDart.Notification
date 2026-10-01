using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace TexasMediaDart.Notification.Api.Authentication;

public sealed class ServiceApiKeyAuthorizationFilter
    : IAsyncAuthorizationFilter
{
    public const string HeaderName = "X-API-Key";

    private readonly ServiceAuthenticationOptions _options;

    public ServiceApiKeyAuthorizationFilter(
        IOptions<ServiceAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public Task OnAuthorizationAsync(
        AuthorizationFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            context.Result =
                new ObjectResult(
                    new ProblemDetails
                    {
                        Title =
                            "Service authentication is not configured.",
                        Status =
                            StatusCodes.Status500InternalServerError
                    })
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };

            return Task.CompletedTask;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(
                HeaderName,
                out var providedApiKey))
        {
            context.Result = CreateUnauthorizedResult();

            return Task.CompletedTask;
        }

        var providedKey =
            providedApiKey.ToString();

        if (!ApiKeysMatch(
                providedKey,
                _options.ApiKey))
        {
            context.Result = CreateUnauthorizedResult();
        }

        return Task.CompletedTask;
    }

    private static bool ApiKeysMatch(
        string providedApiKey,
        string configuredApiKey)
    {
        if (string.IsNullOrEmpty(providedApiKey) ||
            string.IsNullOrEmpty(configuredApiKey))
        {
            return false;
        }

        var providedBytes =
            Encoding.UTF8.GetBytes(providedApiKey);

        var configuredBytes =
            Encoding.UTF8.GetBytes(configuredApiKey);

        if (providedBytes.Length != configuredBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            providedBytes,
            configuredBytes);
    }

    private static ObjectResult CreateUnauthorizedResult()
    {
        return new ObjectResult(
            new ProblemDetails
            {
                Title = "Unauthorized service request.",
                Detail =
                    "A valid service API key is required.",
                Status =
                    StatusCodes.Status401Unauthorized
            })
        {
            StatusCode =
                StatusCodes.Status401Unauthorized
        };
    }
}