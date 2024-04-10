using Essentials.HttpClient.Cache.Extensions;
using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Dictionaries;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics.Extensions;
using Essentials.HttpClient.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Essentials.Configuration.Extensions;
using Essentials.Serialization.Deserializers;
using Essentials.Serialization.Serializers;
using static Essentials.Serialization.EssentialsSerializersFactory;
using static Essentials.Serialization.EssentialsDeserializersFactory;

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
    /// <param name="loggingOptions">Опции логирования</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        LoggingOptions? loggingOptions = null)
    {
        return services.AtomicConfigureService(
            ref _isConfigured,
            () => services.ConfigureEssentialsHttpClientPrivate(configuration, loggingOptions));
    }

    private static void ConfigureEssentialsHttpClientPrivate(
        this IServiceCollection services,
        IConfiguration configuration,
        LoggingOptions? loggingOptions = null)
    {
        if (loggingOptions is null || !loggingOptions.DisableDefaultLogging)
            new LogSubscriber(loggingOptions).SubscribeToLogEvents();
        
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        
        AddSerializers();
        AddDeserializers();

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

    /// <summary>
    /// Добаляет сериалайзеры
    /// </summary>
    /// <returns></returns>
    private static void AddSerializers()
    {
        AddByTypeAndKey(KnownHttpClientSerializers.XML, () => new XmlSerializer());
        AddByTypeAndKey(KnownHttpClientSerializers.NATIVE_JSON, () => new NativeJsonSerializer());
        AddByTypeAndKey(KnownHttpClientSerializers.NEWTONSOFT_JSON, () => new NewtonsoftJsonSerializer());
    }

    /// <summary>
    /// Добаляет десериалайзеры
    /// </summary>
    /// <returns></returns>
    private static void AddDeserializers()
    {
        AddByTypeAndKey(KnownHttpClientDeserializers.XML, () => new XmlDeserializer());
        AddByTypeAndKey(KnownHttpClientDeserializers.NATIVE_JSON, () => new NativeJsonDeserializer());
        AddByTypeAndKey(KnownHttpClientDeserializers.NEWTONSOFT_JSON, () => new NewtonsoftJsonDeserializer());
    }
}