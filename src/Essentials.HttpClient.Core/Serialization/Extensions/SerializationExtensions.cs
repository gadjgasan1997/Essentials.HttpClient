using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Serialization.Extensions;

/// <summary>
/// Методы расширения для сериализации/десериализации
/// </summary>
internal static class SerializationExtensions
{
    /// <summary>
    /// Серилизует объект в строку
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <param name="content">Объект</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns></returns>
    public static Validation<Error, string> SerializeObject<T>(
        this IEssentialsSerializer serializer,
        T content)
    {
        // TODO Log
        return TryOption(() => serializer.Serialize(content))
            .Match(
                Some: Success<Error, string>,
                Fail: exception => Error.New(exception),
                None: () => Error.New($"Результат после серилизации равен null. Объект: '{content}'"));
    }
    
    /// <summary>
    /// Десерилизует строку в объект
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <param name="string">Строка</param>
    /// <typeparam name="TData">Тип объекта</typeparam>
    /// <returns></returns>
    public static Validation<Error, TData> DeserializeString<TData>(
        this IEssentialsDeserializer deserializer,
        string @string)
    {
        // TODO Log
        return TryOption(() => deserializer.Deserialize<TData>(@string))
            .Match(
                Some: Success<Error, TData>,
                Fail: exception => Error.New(exception),
                None: () => Error.New($"Результат после десерилизации равен null. Строка: '{@string}'"));
    }
}