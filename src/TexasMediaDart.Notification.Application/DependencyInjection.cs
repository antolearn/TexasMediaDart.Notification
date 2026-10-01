using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Notification.Application.Notifications.EmailVerification;
using TexasMediaDart.Notification.Application.Notifications.UserInvitation;

namespace TexasMediaDart.Notification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            SendEmailVerificationCommandHandler>();
        services.AddScoped<
            SendUserInvitationNotificationCommandHandler>();

        return services;
    }
}