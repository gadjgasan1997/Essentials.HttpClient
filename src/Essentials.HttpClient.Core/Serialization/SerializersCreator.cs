using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Logging.LogManager;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Класс для создания сериалайзеров
/// </summary>
internal static class SerializersCreator
{
    /// <summary>
    /// Возвращает сериалайзер
    /// </summary>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    public static Validation<Error, TSerializer> GetSerializer<TSerializer>()
        where TSerializer : IEssentialsSerializer
    {
        return Try(EssentialsSerializersFactory.TryGet<TSerializer>)
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
        return Try(EssentialsSerializersFactory.TryGet<TSerializer>(key))
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
        return Try(EssentialsDeserializersFactory.TryGet<TDeserializer>)
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
        return Try(EssentialsDeserializersFactory.TryGet<TDeserializer>(key))
            .ToValidation(exception =>
            {
                var errorMessage = $"Во время получения десериалайзера с типом '{typeof(TDeserializer).FullName}' " +
                                   $"и ключом '{key}' произошло исключение";
                
                MainLogger.Error(exception, errorMessage);
                return Error.New(errorMessage, exception);
            });
    }
}