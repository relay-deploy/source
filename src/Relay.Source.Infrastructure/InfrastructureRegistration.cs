using Microsoft.Extensions.DependencyInjection;
using Relay.Source.Application.Detection;
using Relay.Source.Infrastructure.Compose;

namespace Relay.Source.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddSourceInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IComposeParser, YamlComposeParser>();
        return services;
    }
}
