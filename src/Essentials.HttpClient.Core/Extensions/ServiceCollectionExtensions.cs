using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Clients.Implementations;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Essentials.HttpClient.Dictionaries.KnownMediaTypes;
using static Essentials.HttpClient.Serialization.SerializersCreator;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureEssentialsHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        List<(string, IEssentialsSerializer)>? serializers = null,
        List<(string, IEssentialsDeserializer)>? deserializers = null)
    {
        services.AddHttpClient(nameof(IEssentialsHttpClient));
        services.AddTransient<IEssentialsHttpClient, EssentialsHttpClient>();
        services.AddTransient<IGetHttpClient, EssentialsHttpClient>();
        services.AddTransient<IPostHttpClient, EssentialsHttpClient>();
        
        return services
            .AddOrUpdateSerializers(serializers)
            .AddOrUpdateDeserializers(deserializers);
    }

    /// <summary>
    /// Добаляет или изменяет сериалайзеры
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serializers">Список сериалайзеров</param>
    /// <returns></returns>
    private static IServiceCollection AddOrUpdateSerializers(
        this IServiceCollection services,
        List<(string, IEssentialsSerializer)>? serializers)
    {
        AddOrUpdateSerializer(JSON, new NewtonsoftJsonSerializer());
        AddOrUpdateSerializer(XML, new XmlSerializer());
        serializers?.ForEach(tuple => AddOrUpdateSerializer(tuple.Item1, tuple.Item2));
        
        return services;
    }

    /// <summary>
    /// Добаляет или изменяет десериалайзеры
    /// </summary>
    /// <param name="services"></param>
    /// <param name="deserializers">Список десериалайзеров</param>
    /// <returns></returns>
    private static IServiceCollection AddOrUpdateDeserializers(
        this IServiceCollection services,
        List<(string, IEssentialsDeserializer)>? deserializers)
    {
        AddOrUpdateDeserializer(JSON, new NewtonsoftJsonSerializer());
        AddOrUpdateDeserializer(XML, new XmlSerializer());
        deserializers?.ForEach(tuple => AddOrUpdateDeserializer(tuple.Item1, tuple.Item2));
        
        return services;
    }
}