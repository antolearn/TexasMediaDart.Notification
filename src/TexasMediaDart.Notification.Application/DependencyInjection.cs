using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Notification.Application.Notifications.EmailVerification;

namespace TexasMediaDart.Notification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            SendEmailVerificationCommandHandler>();

        return services;
    }
}