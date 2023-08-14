using Essentials.HttpClient.Extensions;
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
    /// Серилизует объект в поток
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <param name="content">Объект</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Поток</returns>
    public static Validation<Error, Stream> SerializeObject<T>(
        this IEssentialsSerializer serializer,
        T content)
    {
        // TODO Log
        return TryOption(() => serializer.Serialize(content))
            .Match(
                Some: Success<Error, Stream>,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Результат после серилизации равен null"));
    }
    
    /// <summary>
    /// Десерилизует поток в объект
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <param name="stream">Строка</param>
    /// <typeparam name="TData">Тип объекта</typeparam>
    /// <returns></returns>
    public static Validation<Error, TData> DeserializeStream<TData>(
        this IEssentialsDeserializer deserializer,
        Stream stream)
    {
        // TODO Log
        return TryOption(stream.ToByteArray)
            .Bind(bytes => TryOption(() => deserializer.Deserialize<TData>(bytes)))
            .Match(
                Some: Success<Error, TData>,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Результат после десерилизации равен null"));
    }
}