using Essentials.HttpClient.Cache.Extensions;
using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Metrics.Extensions;
using Essentials.HttpClient.Options;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Configuration;
using Essentials.HttpClient.Serialization;
using Essentials.Configuration.Extensions;
using Essentials.HttpClient.Events.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    private static uint _isConfigured;
    
    /// <summary>
    /// Настраивает Http клиент
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration">Опции конфигурации</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AtomicConfigureService(
            ref _isConfigured,
            () => services.ConfigureEssentialsHttpClientPrivate(configuration));
    }
    
    /// <summary>
    /// Настраивает Http клиент
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration">Опции конфигурации</param>
    /// <param name="configureHttpClientAction">Действие по конфигурации клиента</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EssentialsHttpClientConfigurator> configureHttpClientAction)
    {
        return services.AtomicConfigureService(
            ref _isConfigured,
            () =>
            {
                configureHttpClientAction(new EssentialsHttpClientConfigurator());
                services.ConfigureEssentialsHttpClientPrivate(configuration);
            });
    }

    private static void ConfigureEssentialsHttpClientPrivate(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        
        RequestsTimerSubscriber.Subscribe();
        LogSubscriber.Subscribe();
        EventsSubscriber.Subscribe();
        
        SerializersManager.RegisterSerializers();
        SerializersManager.RegisterDeserializers();

        var options = new ClientsOptions();
        var section = configuration.GetSection(ClientsOptions.Section);
        section.Bind(options);
        
        services.TryAdd(
            ServiceDescriptor.Describe(
                serviceType: typeof(IEssentialsHttpClient),
                implementationType: typeof(EssentialsHttpClient),
                lifetime: options.ServiceLifetime ?? ServiceLifetime.Transient));
        
        services.ConfigureMetrics(options.Metrics).ConfigureCache(options.Cache);
        services.AddHostedService<HttpClientHostedService>();
    }
}