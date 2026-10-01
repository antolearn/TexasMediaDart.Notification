using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using TexasMediaDart.Notification.Application.Abstractions.Email;

namespace TexasMediaDart.Notification.Infrastructure.Email;

public sealed class AzureCommunicationEmailSender : IEmailSender
{
    private readonly EmailClient _emailClient;
    private readonly EmailOptions _options;

    public AzureCommunicationEmailSender(
        IOptions<EmailOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException(
                "Email:ConnectionString is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.SenderAddress))
        {
            throw new InvalidOperationException(
                "Email:SenderAddress is not configured.");
        }

        _emailClient =
            new EmailClient(_options.ConnectionString);
    }

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string htmlContent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new ArgumentException(
                "Recipient email is required.",
                nameof(recipientEmail));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException(
                "Email subject is required.",
                nameof(subject));
        }

        var content =
            new EmailContent(subject)
            {
                Html = htmlContent
            };

        var recipients =
            new EmailRecipients(
                new[]
                {
                    new EmailAddress(recipientEmail)
                });

        var message =
            new EmailMessage(
                _options.SenderAddress,
                recipients,
                content);

        await _emailClient.SendAsync(
            WaitUntil.Completed,
            message,
            cancellationToken);
    }
}