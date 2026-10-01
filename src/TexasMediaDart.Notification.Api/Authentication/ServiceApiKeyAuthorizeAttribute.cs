using Microsoft.AspNetCore.Mvc;

namespace TexasMediaDart.Notification.Api.Authentication;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true)]
public sealed class ServiceApiKeyAuthorizeAttribute
    : TypeFilterAttribute
{
    public ServiceApiKeyAuthorizeAttribute()
        : base(typeof(ServiceApiKeyAuthorizationFilter))
    {
    }
}