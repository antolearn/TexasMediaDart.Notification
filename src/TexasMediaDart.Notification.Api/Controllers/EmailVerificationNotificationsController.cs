using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Notification.Api.Models.Notifications;
using TexasMediaDart.Notification.Application.Notifications.EmailVerification;
using TexasMediaDart.Notification.Api.Authentication;

namespace TexasMediaDart.Notification.Api.Controllers;

[ApiController]
[Route("internal/notifications/email-verification")]
[ServiceApiKeyAuthorize]
public sealed class EmailVerificationNotificationsController
    : ControllerBase
{
    private readonly SendEmailVerificationCommandHandler _handler;

    public EmailVerificationNotificationsController(
        SendEmailVerificationCommandHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send(
        [FromBody] EmailVerificationNotificationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command =
                new SendEmailVerificationCommand(
                    request.RecipientEmail,
                    request.VerificationUrl);

            await _handler.HandleAsync(
                command,
                cancellationToken);

            return Accepted(
                new
                {
                    message =
                        "Email verification notification submitted successfully."
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title =
                        "Email verification notification validation failed.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
        }
    }
}