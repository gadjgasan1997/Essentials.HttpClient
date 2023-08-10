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
    private static readonly List<ISerializer> _serializers = new();

    /// <summary>
    /// Список десериалайзеров
    /// </summary>
    private static readonly List<IDeserializer> _deserializers = new();
    
    /// <summary>
    /// Добавляет или заменяет сериалайзер
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    public static void AddOrUpdateSerializer(ISerializer serializer)
    {
        var existingSerializer = _serializers.FirstOrDefault(essentialsSerializer =>
            essentialsSerializer.GetType() == typeof(ISerializer));
        
        if (existingSerializer is null)
        {
            _serializers.Add(serializer);
            return;
        }

        var index = _serializers.IndexOf(existingSerializer);
        _serializers[index] = serializer;
    }
    
    /// <summary>
    /// Добавляет или заменяет десериалайзер
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    public static void AddOrUpdateDeserializer(IDeserializer deserializer)
    {
        var existingDeserializer = _deserializers.FirstOrDefault(essentialsDeserializer =>
            essentialsDeserializer.GetType() == typeof(IDeserializer));
        
        if (existingDeserializer is null)
        {
            _deserializers.Add(deserializer);
            return;
        }

        var index = _deserializers.IndexOf(existingDeserializer);
        _deserializers[index] = deserializer;
    }
    
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, ISerializer> GetSerializer<TSerializer>()
    {
        // TODO Log
        return Try(() => _serializers.First(serializer => serializer is TSerializer))
            .ToValidation(exception =>
                Error.New($"Во время получения сериалайзера с типом '{typeof(TSerializer).FullName}' произошло исключение",
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
        return Try(() => _deserializers.First(deserializer => deserializer is TDeserializer))
            .ToValidation(exception =>
                Error.New($"Во время получения десериалайзера с типом '{typeof(TDeserializer).FullName}' произошло исключение",
                    exception));
    }
}