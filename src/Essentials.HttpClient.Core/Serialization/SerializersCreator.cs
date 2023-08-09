using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Essentials.Func.Utils.Extensions;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.KnownMediaTypes;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Класс для создания сериалайзеров
/// </summary>
internal static class SerializersCreator
{
    ///<summary>
    /// Мапа названий типа контента на сериалайзеры
    /// </summary>
    private static readonly ConcurrentDictionary<string, IEssentialsSerializer> _serializersMap = new();
    
    ///<summary>
    /// Мапа названий типа контента на десериалайзеры
    /// </summary>
    private static readonly ConcurrentDictionary<string, IEssentialsDeserializer> _deserializersMap = new();

    /// <summary>
    /// Добавляет или заменяет сериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <param name="serializer">Сериалайзер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOrUpdateSerializer(string contentType, IEssentialsSerializer serializer)
    {
        var resolvedType = ResolveContentType(contentType);
        if (string.IsNullOrWhiteSpace(resolvedType))
            throw new ArgumentNullException(nameof(contentType));

        if (!CheckContentType(resolvedType))
            return;

        if (_serializersMap.ContainsKey(resolvedType))
        {
            _serializersMap[resolvedType] = serializer;
            return;
        }

        _serializersMap.TryAdd(resolvedType, serializer);
    }
    
    /// <summary>
    /// Добавляет или заменяет десериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <param name="deserializer">Десериалайзер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOrUpdateDeserializer(string contentType, IEssentialsDeserializer deserializer)
    {
        var resolvedType = ResolveContentType(contentType);
        if (string.IsNullOrWhiteSpace(resolvedType))
            throw new ArgumentNullException(nameof(contentType));

        if (!CheckContentType(resolvedType))
            return;

        if (_deserializersMap.ContainsKey(resolvedType))
        {
            _deserializersMap[resolvedType] = deserializer;
            return;
        }

        _deserializersMap.TryAdd(resolvedType, deserializer);
    }

    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns></returns>
    public static Validation<Error, IEssentialsSerializer> GetSerializer(string contentType)
    {
        try
        {
            var key = ResolveContentType(contentType);
            return Success<Error, IEssentialsSerializer>(_serializersMap[key]);
        }
        catch (Exception ex)
        {
            // TODO Log
            return Error.New(ex);
        }
    }

    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns></returns>
    public static Validation<Error, IEssentialsDeserializer> GetDeserializer(string contentType)
    {
        try
        {
            var key = ResolveContentType(contentType);
            return Success<Error, IEssentialsDeserializer>(_deserializersMap[key]);
        }
        catch (Exception ex)
        {
            // TODO Log
            return Error.New(ex);
        }
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

    /// <summary>
    /// Возвращает подходящий тип содержимого
    /// </summary>
    /// <param name="contentType">Исходный тип содержимого</param>
    /// <returns></returns>
    private static string ResolveContentType(string contentType)
    {
        var type = contentType.FullTrim().ToLower();
        return type switch
        {
            "json" => JSON,
            "xml" => XML,
            _ => type
        };
    }
}