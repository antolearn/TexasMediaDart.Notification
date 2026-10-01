namespace TexasMediaDart.Notification.Api.Authentication;

public sealed class ServiceAuthenticationOptions
{
    public const string SectionName = "ServiceAuthentication";

    public string ApiKey { get; init; } = string.Empty;
}