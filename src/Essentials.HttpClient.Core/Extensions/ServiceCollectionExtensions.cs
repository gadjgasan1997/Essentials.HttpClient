using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Clients.Implementations;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Essentials.HttpClient.Dictionaries.KnownMediaTypes;
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
        List<(string, IEssentialsSerializer)>? serializers = null,
        List<(string, IEssentialsDeserializer)>? deserializers = null)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        services.TryAddTransient<IEssentialsHttpClient, EssentialsHttpClient>();
        services.TryAddTransient<IGetHttpClient, EssentialsHttpClient>();
        services.TryAddTransient<IPostHttpClient, EssentialsHttpClient>();
        
        AddOrUpdateSerializers(serializers);
        AddOrUpdateDeserializers(deserializers);

        return services;
    }

    /// <summary>
    /// Добаляет или изменяет сериалайзеры
    /// </summary>
    /// <param name="serializers">Список сериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateSerializers(List<(string, IEssentialsSerializer)>? serializers)
    {
        AddOrUpdateSerializer(JSON, new NewtonsoftJsonSerializer());
        AddOrUpdateSerializer(XML, new XmlSerializer());
        serializers?.ForEach(tuple => AddOrUpdateSerializer(tuple.Item1, tuple.Item2));
    }

    /// <summary>
    /// Добаляет или изменяет десериалайзеры
    /// </summary>
    /// <param name="deserializers">Список десериалайзеров</param>
    /// <returns></returns>
    private static void AddOrUpdateDeserializers(List<(string, IEssentialsDeserializer)>? deserializers)
    {
        AddOrUpdateDeserializer(JSON, new NewtonsoftJsonSerializer());
        AddOrUpdateDeserializer(XML, new XmlSerializer());
        deserializers?.ForEach(tuple => AddOrUpdateDeserializer(tuple.Item1, tuple.Item2));
    }
}