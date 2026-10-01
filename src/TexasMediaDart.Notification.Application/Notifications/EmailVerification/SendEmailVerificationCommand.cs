namespace TexasMediaDart.Notification.Application.Notifications.EmailVerification;

public sealed record SendEmailVerificationCommand(
    string RecipientEmail,
    string VerificationUrl);