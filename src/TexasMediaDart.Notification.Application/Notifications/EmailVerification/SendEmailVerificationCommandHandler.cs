using System.Net;
using TexasMediaDart.Notification.Application.Abstractions.Email;

namespace TexasMediaDart.Notification.Application.Notifications.EmailVerification;

public sealed class SendEmailVerificationCommandHandler
{
    private readonly IEmailSender _emailSender;

    public SendEmailVerificationCommandHandler(
        IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task HandleAsync(
        SendEmailVerificationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.RecipientEmail))
        {
            throw new ArgumentException(
                "Recipient email is required.",
                nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.VerificationUrl))
        {
            throw new ArgumentException(
                "Verification URL is required.",
                nameof(command));
        }

        if (!Uri.TryCreate(
                command.VerificationUrl,
                UriKind.Absolute,
                out var verificationUri))
        {
            throw new ArgumentException(
                "Verification URL must be a valid absolute URL.",
                nameof(command));
        }

        if (verificationUri.Scheme != Uri.UriSchemeHttp &&
            verificationUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException(
                "Verification URL must use HTTP or HTTPS.",
                nameof(command));
        }

        var safeVerificationUrl =
            WebUtility.HtmlEncode(command.VerificationUrl);

        const string subject =
            "Verify your TexasDart email address";

        var htmlContent =
            $$"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif;
                         color: #333333;
                         line-height: 1.6;">

                <div style="max-width: 600px;
                            margin: 0 auto;
                            padding: 24px;">

                    <h2>
                        Verify your email address
                    </h2>

                    <p>
                        Thank you for creating your TexasDart account.
                    </p>

                    <p>
                        Please verify your email address before signing in.
                    </p>

                    <p style="margin: 32px 0;">
                        <a href="{{safeVerificationUrl}}"
                           style="display: inline-block;
                                  padding: 12px 24px;
                                  background-color: #f57c00;
                                  color: #ffffff;
                                  text-decoration: none;
                                  border-radius: 4px;">
                            Verify Email Address
                        </a>
                    </p>

                    <p>
                        If the button does not work, copy and paste this
                        link into your browser:
                    </p>

                    <p style="word-break: break-all;">
                        <a href="{{safeVerificationUrl}}">
                            {{safeVerificationUrl}}
                        </a>
                    </p>

                    <p>
                        If you did not create a TexasDart account,
                        you can ignore this email.
                    </p>

                    <p>
                        TexasDart
                    </p>

                </div>

            </body>
            </html>
            """;

        await _emailSender.SendAsync(
            command.RecipientEmail.Trim(),
            subject,
            htmlContent,
            cancellationToken);
    }
}