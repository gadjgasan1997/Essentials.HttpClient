using System.Collections.Concurrent;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using ISerializer = Essentials.HttpClient.Serialization.IEssentialsSerializer;
using IDeserializer = Essentials.HttpClient.Serialization.IEssentialsDeserializer;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Класс для создания сериалайзеров
/// </summary>
internal static class SerializersCreator
{
    /// <summary>
    /// Список сериалайзеров
    /// </summary>
    private static readonly ConcurrentDictionary<string, ISerializer> _serializers = new();

    /// <summary>
    /// Список десериалайзеров
    /// </summary>
    private static readonly ConcurrentDictionary<string, IDeserializer> _deserializers = new();

    /// <summary>
    /// Добавляет или заменяет сериалайзер
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    public static void AddOrUpdateSerializer(ISerializer serializer)
    {
        // TODO Log
        var key = serializer.GetType().FullName!;
        if (!_serializers.TryGetValue(key, out var existingSerializer))
        {
            _serializers.TryAdd(key, serializer);
            return;
        }

        _serializers.TryUpdate(key, serializer, existingSerializer);
    }
    
    /// <summary>
    /// Добавляет или заменяет десериалайзер
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    public static void AddOrUpdateDeserializer(IDeserializer deserializer)
    {
        // TODO Log
        var key = deserializer.GetType().FullName!;
        if (!_deserializers.TryGetValue(key, out var existingDeserializer))
        {
            _deserializers.TryAdd(key, deserializer);
            return;
        }

        _deserializers.TryUpdate(key, deserializer, existingDeserializer);
    }
    
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, ISerializer> GetSerializer<TSerializer>()
    {
        // TODO Log
        var key = typeof(TSerializer).FullName!;
        return Try(() => _serializers[key])
            .ToValidation(exception =>
                Error.New($"Во время получения сериалайзера с ключом '{key}' произошло исключение",
                    exception));
    }
    
    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, IDeserializer> GetDeserializer<TDeserializer>()
    {
        // TODO Log
        var key = typeof(TDeserializer).FullName!;
        return Try(() => _deserializers[key])
            .ToValidation(exception =>
                Error.New($"Во время получения десериалайзера с ключом '{key}' произошло исключение",
                    exception));
    }
}