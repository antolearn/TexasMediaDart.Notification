using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Notification.Application.Abstractions.Email;
using TexasMediaDart.Notification.Infrastructure.Email;

namespace TexasMediaDart.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailOptions>(
            configuration.GetSection(
                EmailOptions.SectionName));

        var emailProvider =
            configuration["Email:Provider"]
            ?? throw new InvalidOperationException(
                "Email:Provider is not configured.");

        switch (emailProvider)
        {
            case "AzureCommunicationServices":
                services.AddSingleton<
                    IEmailSender,
                    AzureCommunicationEmailSender>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported email provider: {emailProvider}");
        }

        return services;
    }
}