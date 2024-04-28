using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization;
using Essentials.Serialization.Deserializers;
using Essentials.Serialization.Serializers;
using Essentials.HttpClient.Dictionaries;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Logging.LogManager;
using static Essentials.Serialization.EssentialsSerializersFactory;
using static Essentials.Serialization.EssentialsDeserializersFactory;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Менеджер для управления сериалайзерами
/// </summary>
internal static class SerializersManager
{
    public static XmlSerializer? XmlSerializer = null;
    public static NativeJsonSerializer? NativeJsonSerializer = null;
    public static NewtonsoftJsonSerializer? NewtonsoftJsonSerializer = null;
    
    public static XmlDeserializer? XmlDeserializer = null;
    public static NativeJsonDeserializer? NativeJsonDeserializer = null;
    public static NewtonsoftJsonDeserializer? NewtonsoftJsonDeserializer = null;
    
    /// <summary>
    /// Регистрирует сериалайзеры
    /// </summary>
    /// <returns></returns>
    public static void RegisterSerializers()
    {
        AddOrUpdateByTypeAndKey(
            KnownHttpClientSerializers.XML,
            func: () => XmlSerializer ?? new XmlSerializer());
        
        AddOrUpdateByTypeAndKey(
            KnownHttpClientSerializers.NATIVE_JSON,
            func: () => NativeJsonSerializer ?? new NativeJsonSerializer());
        
        AddOrUpdateByTypeAndKey(
            KnownHttpClientSerializers.NEWTONSOFT_JSON,
            func: () => NewtonsoftJsonSerializer ?? new NewtonsoftJsonSerializer());
    }

    /// <summary>
    /// Регистрирует десериалайзеры
    /// </summary>
    /// <returns></returns>
    public static void RegisterDeserializers()
    {
        AddOrUpdateByTypeAndKey(
            KnownHttpClientDeserializers.XML,
            func: () => XmlDeserializer ?? new XmlDeserializer());
        
        AddOrUpdateByTypeAndKey(
            KnownHttpClientDeserializers.NATIVE_JSON,
            func: () => NativeJsonDeserializer ?? new NativeJsonDeserializer());
        
        AddOrUpdateByTypeAndKey(
            KnownHttpClientDeserializers.NEWTONSOFT_JSON,
            func: () => NewtonsoftJsonDeserializer ?? new NewtonsoftJsonDeserializer());
    }
    
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, TSerializer> GetSerializer<TSerializer>()
        where TSerializer : IEssentialsSerializer
    {
        return Try(TryGet<TSerializer>)
            .ToValidation(exception =>
            {
                var errorMessage = $"Во время получения сериалайзера с типом " +
                                   $"'{typeof(TSerializer).FullName}' произошло исключение";
                
                MainLogger.Error(exception, errorMessage);
                return Error.New(errorMessage, exception);
            });
    }
    
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, TSerializer> GetSerializer<TSerializer>(string key)
        where TSerializer : IEssentialsSerializer
    {
        return Try(TryGet<TSerializer>(key))
            .ToValidation(exception =>
            {
                var errorMessage = $"Во время получения сериалайзера с типом '{typeof(TSerializer).FullName}' " +
                                   $"и ключом '{key}' произошло исключение";
                
                MainLogger.Error(exception, errorMessage);
                return Error.New(errorMessage, exception);
            });
    }
    
    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, TDeserializer> GetDeserializer<TDeserializer>()
        where TDeserializer : IEssentialsDeserializer
    {
        return Try(TryGet<TDeserializer>)
            .ToValidation(exception =>
            {
                var errorMessage = $"Во время получения десериалайзера с типом " +
                                   $"'{typeof(TDeserializer).FullName}' произошло исключение";
                
                MainLogger.Error(exception, errorMessage);
                return Error.New(errorMessage, exception);
            });
    }
    
    /// <summary>
    /// Возвращает десериалайзер
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, TDeserializer> GetDeserializer<TDeserializer>(string key)
        where TDeserializer : IEssentialsDeserializer
    {
        return Try(TryGet<TDeserializer>(key))
            .ToValidation(exception =>
            {
                var errorMessage = $"Во время получения десериалайзера с типом '{typeof(TDeserializer).FullName}' " +
                                   $"и ключом '{key}' произошло исключение";
                
                MainLogger.Error(exception, errorMessage);
                return Error.New(errorMessage, exception);
            });
    }
}