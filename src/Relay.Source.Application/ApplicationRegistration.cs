using Microsoft.Extensions.DependencyInjection;
using Relay.Source.Application.Detection;

namespace Relay.Source.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddSourceApplication(this IServiceCollection services)
    {
        services.AddScoped<ManifestLoader>();
        services.AddScoped<DetectRepository>();
        return services;
    }
}
