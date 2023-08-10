using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Essentials.HttpClient.ContentTypes.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Класс для создания сериалайзеров
/// </summary>
internal static class SerializersCreator
{
    ///<summary>
    /// Мапа названий типа контента на сериалайзеры
    /// </summary>
    private static readonly ConcurrentDictionary<IContentType, IEssentialsSerializer> _serializersMap = new();
    
    ///<summary>
    /// Мапа названий типа контента на десериалайзеры
    /// </summary>
    private static readonly ConcurrentDictionary<IContentType, IEssentialsDeserializer> _deserializersMap = new();

    /// <summary>
    /// Добавляет или заменяет сериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <param name="serializer">Сериалайзер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOrUpdateSerializer(IContentType contentType, IEssentialsSerializer serializer)
    {
        if (!CheckContentType(contentType.ContentTypeName))
            return;

        if (_serializersMap.ContainsKey(contentType))
        {
            _serializersMap[contentType] = serializer;
            return;
        }

        _serializersMap.TryAdd(contentType, serializer);
    }
    
    /// <summary>
    /// Добавляет или заменяет десериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <param name="deserializer">Десериалайзер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOrUpdateDeserializer(IContentType contentType, IEssentialsDeserializer deserializer)
    {
        if (!CheckContentType(contentType.ContentTypeName))
            return;

        if (_deserializersMap.ContainsKey(contentType))
        {
            _deserializersMap[contentType] = deserializer;
            return;
        }

        _deserializersMap.TryAdd(contentType, deserializer);
    }

    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns></returns>
    public static Validation<Error, IEssentialsSerializer> GetSerializer(IContentType contentType)
    {
        // TODO Log
        return Try(() => _serializersMap[contentType])
            .ToValidation(exception =>
                Error.New($"Во время получения сериалайзера для содержимого с типом '{contentType}' произошло исключение",
                    exception));
    }

    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns></returns>
    public static Validation<Error, IEssentialsDeserializer> GetDeserializer(IContentType contentType)
    {
        // TODO Log
        return Try(() => _deserializersMap[contentType])
            .ToValidation(exception =>
                Error.New($"Во время получения десериалайзера для содержимого с типом '{contentType}' произошло исключение",
                    exception));
    }

    /// <summary>
    /// Проверяет тип содержимого
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns></returns>
    private static bool CheckContentType(string contentType)
    {
        try
        {
            _ = new MediaTypeHeaderValue(contentType);
            return true;
        }
        catch (FormatException ex)
        {
            // TODO Log
            return false;
        }
    }
}