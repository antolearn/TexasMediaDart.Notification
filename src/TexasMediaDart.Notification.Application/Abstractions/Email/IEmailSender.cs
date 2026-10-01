namespace TexasMediaDart.Notification.Application.Abstractions.Email;

public interface IEmailSender
{
    Task SendAsync(
        string recipientEmail,
        string subject,
        string htmlContent,
        CancellationToken cancellationToken = default);
}