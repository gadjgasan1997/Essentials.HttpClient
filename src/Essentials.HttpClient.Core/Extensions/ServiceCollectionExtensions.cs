using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Clients.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        services.AddTransient<IEssentialsHttpClient, EssentialsHttpClient>();
        services.AddTransient<IGetHttpClient, EssentialsHttpClient>();
        services.AddTransient<IPostHttpClient, EssentialsHttpClient>();
        
        return services;
    }
}