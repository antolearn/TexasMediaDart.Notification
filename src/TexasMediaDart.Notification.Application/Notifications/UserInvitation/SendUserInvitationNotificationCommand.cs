namespace TexasMediaDart.Notification.Application.Notifications.UserInvitation;

public sealed record SendUserInvitationNotificationCommand(
    string RecipientEmail,
    string InvitationUrl,
    DateTime ExpiresUtc);