using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Notification.Api.Authentication;
using TexasMediaDart.Notification.Api.Models.Notifications;
using TexasMediaDart.Notification.Application.Notifications.UserInvitation;

namespace TexasMediaDart.Notification.Api.Controllers;

[ApiController]
[Route("internal/notifications/user-invitation")]
[ServiceApiKeyAuthorize]
public sealed class UserInvitationNotificationsController
    : ControllerBase
{
    private readonly SendUserInvitationNotificationCommandHandler
        _handler;

    public UserInvitationNotificationsController(
        SendUserInvitationNotificationCommandHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Send(
        [FromBody] UserInvitationNotificationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command =
                new SendUserInvitationNotificationCommand(
                    request.RecipientEmail,
                    request.InvitationUrl,
                    request.ExpiresUtc);

            await _handler.HandleAsync(
                command,
                cancellationToken);

            return Accepted(
                new
                {
                    message =
                        "User invitation notification submitted successfully."
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title =
                        "User invitation notification validation failed.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
        }
    }
}