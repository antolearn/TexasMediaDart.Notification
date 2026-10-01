using System.Net;
using TexasMediaDart.Notification.Application.Abstractions.Email;

namespace TexasMediaDart.Notification.Application.Notifications.UserInvitation;

public sealed class SendUserInvitationNotificationCommandHandler
{
    private readonly IEmailSender _emailSender;

    public SendUserInvitationNotificationCommandHandler(
        IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task HandleAsync(
        SendUserInvitationNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            command.RecipientEmail);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            command.InvitationUrl);

        var encodedInvitationUrl =
            WebUtility.HtmlEncode(command.InvitationUrl);

        const string subject =
            "You're invited to TexasMediaDart";

        var htmlContent =
            $"""
            <!DOCTYPE html>
            <html>
            <body style="
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333333;">

                <div style="
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 24px;">

                    <h2>
                        You're invited to TexasMediaDart
                    </h2>

                    <p>
                        You have been invited to join
                        TexasMediaDart.
                    </p>

                    <p>
                        Click the button below to accept your
                        invitation and complete your account setup.
                    </p>

                    <p style="margin: 28px 0;">
                        <a
                            href="{encodedInvitationUrl}"
                            style="
                                display: inline-block;
                                padding: 12px 24px;
                                background-color: #F57C00;
                                color: #ffffff;
                                text-decoration: none;
                                border-radius: 4px;
                                font-weight: bold;">
                            Accept Invitation
                        </a>
                    </p>

                    <p>
                        This invitation expires on
                        <strong>
                            {command.ExpiresUtc:yyyy-MM-dd HH:mm} UTC
                        </strong>.
                    </p>

                    <p>
                        If the button does not work, copy and paste
                        this link into your browser:
                    </p>

                    <p style="
                        word-break: break-all;
                        color: #555555;">
                        {encodedInvitationUrl}
                    </p>

                    <hr style="
                        border: 0;
                        border-top: 1px solid #dddddd;
                        margin: 24px 0;" />

                    <p style="
                        font-size: 13px;
                        color: #666666;">
                        If you were not expecting this invitation,
                        you can safely ignore this email.
                    </p>

                    <p>
                        <strong>TexasDart</strong><br />
                        Target bullseye with us!!
                    </p>

                </div>

            </body>
            </html>
            """;

        await _emailSender.SendAsync(
            command.RecipientEmail,
            subject,
            htmlContent,
            cancellationToken);
    }
}