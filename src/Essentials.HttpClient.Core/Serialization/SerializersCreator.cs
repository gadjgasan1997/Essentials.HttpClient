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
    /// <param name="serializerInfo">Информация о сериалайзере</param>
    public static void AddOrUpdateSerializer(SerializerInfo serializerInfo)
    {
        // TODO Log
        var key = serializerInfo.GetKey();
        if (!_serializers.TryGetValue(key, out var existingSerializer))
        {
            _serializers.TryAdd(key, serializerInfo.Serializer);
            return;
        }

        _serializers.TryUpdate(key, serializerInfo.Serializer, existingSerializer);
    }
    
    /// <summary>
    /// Добавляет или заменяет десериалайзер
    /// </summary>
    /// <param name="serializerInfo">Информация о десериалайзере</param>
    public static void AddOrUpdateDeserializer(SerializerInfo serializerInfo)
    {
        // TODO Log
        var key = serializerInfo.GetKey();
        if (!_deserializers.TryGetValue(key, out var existingDeserializer))
        {
            _deserializers.TryAdd(key, serializerInfo.Serializer);
            return;
        }

        _deserializers.TryUpdate(key, serializerInfo.Serializer, existingDeserializer);
    }
    
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <param name="key">Необязательный ключ, по умолчанию равный полному названию типа сериалайзера</param>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, ISerializer> GetSerializer<TSerializer>(string? key = null)
    {
        // TODO Log
        key ??= typeof(TSerializer).FullName!;
        return Try(() => _serializers[key])
            .ToValidation(exception =>
                Error.New($"Во время получения сериалайзера с ключом '{key}' произошло исключение",
                    exception));
    }
    
    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <param name="key">Необязательный ключ, по умолчанию равный полному названию типа десериалайзера</param>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, IDeserializer> GetDeserializer<TDeserializer>(string? key = null)
    {
        // TODO Log
        key ??= typeof(TDeserializer).FullName!;
        return Try(() => _deserializers[key])
            .ToValidation(exception =>
                Error.New($"Во время получения десериалайзера с ключом '{key}' произошло исключение",
                    exception));
    }
}