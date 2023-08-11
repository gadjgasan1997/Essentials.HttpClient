using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Metrics.Extensions;
using Essentials.HttpClient.Options;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Essentials.HttpClient.Serialization.SerializersCreator;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает Http клиент
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration">Опции конфигурации</param>
    /// <param name="serializers">Список сериалайзеров</param>
    /// <param name="deserializers">Список десериалайзеров</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        List<SerializerInfo>? serializers = null,
        List<SerializerInfo>? deserializers = null)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        
        AddOrUpdateSerializers(serializers);
        AddOrUpdateDeserializers(deserializers);

        var options = new ClientsOptions();
        var section = configuration.GetSection(ClientsOptions.Section);
        section.Bind(options);
        
        services.TryAdd(
            ServiceDescriptor.Describe(
                serviceType: typeof(IEssentialsHttpClient),
                implementationType: typeof(EssentialsHttpClient),
                lifetime: options.ServiceLifetime ?? ServiceLifetime.Transient));
        
        return services.ConfigureMetrics(options.Metrics);
    }

    /// <summary>
    /// Добаляет или изменяет сериалайзеры
    /// </summary>
    /// <param name="serializers">Список сериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateSerializers(List<SerializerInfo>? serializers)
    {
        AddOrUpdateSerializer(new SerializerInfo(new NativeJsonSerializer()));
        AddOrUpdateSerializer(new SerializerInfo(new NewtonsoftJsonSerializer()));
        AddOrUpdateSerializer(new SerializerInfo(new XmlSerializer()));
        
        serializers?.ForEach(AddOrUpdateSerializer);
    }

    /// <summary>
    /// Добаляет или изменяет десериалайзеры
    /// </summary>
    /// <param name="deserializers">Список десериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateDeserializers(List<SerializerInfo>? deserializers)
    {
        AddOrUpdateDeserializer(new SerializerInfo(new NativeJsonSerializer()));
        AddOrUpdateDeserializer(new SerializerInfo(new NewtonsoftJsonSerializer()));
        AddOrUpdateDeserializer(new SerializerInfo(new XmlSerializer()));
        
        deserializers?.ForEach(AddOrUpdateDeserializer);
    }
}