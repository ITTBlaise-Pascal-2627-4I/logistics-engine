using LogisticsEngine.Domain.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Infrastructure;

// Extension method for DI container for earch layer
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDirectory)
    {
        return services.AddSingleton<IHubRepository>(new JsonHubRepository(dataDirectory));
    }
}
