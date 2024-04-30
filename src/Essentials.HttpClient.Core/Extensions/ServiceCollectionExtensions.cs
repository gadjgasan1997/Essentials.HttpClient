using Essentials.HttpClient.Cache.Extensions;
using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Metrics.Extensions;
using Essentials.HttpClient.Options;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Configuration;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Events.Subscribers;
using Essentials.HttpClient.HostedServices;
using Essentials.Configuration.Extensions;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.RequestsInterception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

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
        
        // Крайне важна последовательность регистрации подписчиков на события
        services
            .AddSingleton<BaseEvensSubscriber, LogSubscriber>()
            .AddSingleton<BaseEvensSubscriber, MetricsSubscriber>()
            .AddSingleton<BaseEvensSubscriber, EventsSubscriber>()
            .AddHostedService<EvensSubscriberHostedService>();
        
        SerializersManager.RegisterSerializers();
        SerializersManager.RegisterDeserializers();
        
        services.RegisterInterceptors();

        var options = new ClientsOptions();
        var section = configuration.GetSection(ClientsOptions.Section);
        section.Bind(options);
        
        services.TryAdd(
            ServiceDescriptor.Describe(
                serviceType: typeof(IEssentialsHttpClient),
                implementationType: typeof(EssentialsHttpClient),
                lifetime: options.ServiceLifetime ?? ServiceLifetime.Transient));
        
        services.ConfigureMetrics(options.Metrics).ConfigureCache(options.Cache);
        services.AddHostedService<RegisterHttpClientsHostedService>();
    }

    /// <summary>
    /// Регистриует интерсепторы
    /// </summary>
    /// <param name="services"></param>
    private static void RegisterInterceptors(this IServiceCollection services)
    {
        foreach (var type in InterceptorsStorage.GetInterceptorsToRegister())
        {
            var descriptor = new ServiceDescriptor(typeof(IRequestInterceptor), type, ServiceLifetime.Singleton);
            services.Add(descriptor);
        }
    }
}