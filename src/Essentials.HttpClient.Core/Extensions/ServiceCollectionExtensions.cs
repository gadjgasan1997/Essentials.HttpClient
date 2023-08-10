using Essentials.HttpClient.Clients;
using Essentials.HttpClient.ContentTypes.Interfaces;
using Essentials.HttpClient.Metrics.Extensions;
using Essentials.HttpClient.Options;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Essentials.HttpClient.Serialization.SerializersCreator;
using static Essentials.HttpClient.ContentTypes.Storage;

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
        List<(IContentType, IEssentialsSerializer)>? serializers = null,
        List<(IContentType, IEssentialsDeserializer)>? deserializers = null)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        services.TryAddTransient<IEssentialsHttpClient, EssentialsHttpClient>();
        
        AddOrUpdateSerializers(serializers);
        AddOrUpdateDeserializers(deserializers);

        var options = new ClientsOptions();
        var section = configuration.GetSection(ClientsOptions.Section);
        section.Bind(options);
        
        return services.ConfigureMetrics(options.Metrics);
    }

    /// <summary>
    /// Добаляет или изменяет сериалайзеры
    /// </summary>
    /// <param name="serializers">Список сериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateSerializers(List<(IContentType, IEssentialsSerializer)>? serializers)
    {
        AddOrUpdateSerializer(Application.Json, new NativeJsonSerializer());
        AddOrUpdateSerializer(Application.Xml, new XmlSerializer());
        serializers?.ForEach(tuple => AddOrUpdateSerializer(tuple.Item1, tuple.Item2));
    }

    /// <summary>
    /// Добаляет или изменяет десериалайзеры
    /// </summary>
    /// <param name="deserializers">Список десериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateDeserializers(List<(IContentType, IEssentialsDeserializer)>? deserializers)
    {
        AddOrUpdateDeserializer(Application.Json, new NativeJsonSerializer());
        AddOrUpdateDeserializer(Application.Xml, new XmlSerializer());
        deserializers?.ForEach(tuple => AddOrUpdateDeserializer(tuple.Item1, tuple.Item2));
    }
}