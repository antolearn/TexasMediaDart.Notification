namespace TexasMediaDart.Notification.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Provider { get; init; } = string.Empty;

    public string ConnectionString { get; init; } = string.Empty;

    public string SenderAddress { get; init; } = string.Empty;

    public string SenderDisplayName { get; init; } = string.Empty;

    public string FrontendBaseUrl { get; init; } = string.Empty;
}