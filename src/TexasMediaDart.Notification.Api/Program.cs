using TexasMediaDart.Notification.Api.Authentication;
using TexasMediaDart.Notification.Application;
using TexasMediaDart.Notification.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Services
// ------------------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

// ------------------------------------------------------------
// Service-to-service authentication
// ------------------------------------------------------------

builder.Services.Configure<ServiceAuthenticationOptions>(
    builder.Configuration.GetSection(
        ServiceAuthenticationOptions.SectionName));

builder.Services.AddScoped<
    ServiceApiKeyAuthorizationFilter>();

// ------------------------------------------------------------
// Application
// ------------------------------------------------------------

builder.Services.AddApplication();

// ------------------------------------------------------------
// Infrastructure
// ------------------------------------------------------------

builder.Services.AddInfrastructure(
    builder.Configuration);

// ------------------------------------------------------------
// Build application
// ------------------------------------------------------------

var app = builder.Build();

// ------------------------------------------------------------
// HTTP pipeline
// ------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

// ------------------------------------------------------------
// Health endpoints
// ------------------------------------------------------------

app.MapGet(
    "/health/api",
    () => Results.Ok(
        new
        {
            status = "Healthy",
            service = "TexasMediaDart.Notification.Api",
            utc = DateTime.UtcNow
        }));

app.MapHealthChecks("/health");

// ------------------------------------------------------------
// Run
// ------------------------------------------------------------

app.Run();