namespace TexasMediaDart.Notification.Api.Models.Notifications;

public sealed record EmailVerificationNotificationRequest(
    string RecipientEmail,
    string VerificationUrl);