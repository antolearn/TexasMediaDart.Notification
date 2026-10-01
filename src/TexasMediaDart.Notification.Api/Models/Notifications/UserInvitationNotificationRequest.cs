namespace TexasMediaDart.Notification.Api.Models.Notifications;

public sealed record UserInvitationNotificationRequest(
    string RecipientEmail,
    string InvitationUrl,
    DateTime ExpiresUtc);