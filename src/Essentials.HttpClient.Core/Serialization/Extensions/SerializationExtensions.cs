using Essentials.HttpClient.Events.Args;
using Essentials.HttpClient.Extensions;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Events.EventsPublisher;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

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
    /// <param name="request">Запрос</param>
    /// <param name="content">Объект</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Поток</returns>
    public static Validation<Error, Stream> SerializeObject<T>(
        this IEssentialsSerializer serializer,
        IRequest request,
        T content)
    {
        return TryOption(() => serializer.Serialize(content))
            .Match(
                Success<Error, Stream>,
                None: () => OnNone(),
                Fail: exception => OnFail(exception));

        Error OnNone()
        {
            RaiseOnSerializeError(new SerializeErrorEventArgs(request, content, SerializeNull));
            return Error.New(SerializeNull);
        }

        Error OnFail(Exception exception)
        {
            RaiseOnSerializeError(new SerializeErrorEventArgs(request, content, exception));
            return Error.New(exception);
        }
    }
    
    /// <summary>
    /// Десерилизует поток в объект
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <param name="response">Ответ</param>
    /// <param name="stream">Строка</param>
    /// <typeparam name="TData">Тип объекта</typeparam>
    /// <returns></returns>
    public static Validation<Error, TData> DeserializeStream<TData>(
        this IEssentialsDeserializer deserializer,
        IResponse response,
        Stream stream)
    {
        return TryOption(stream.ToByteArray)
            .Bind(bytes => TryOption(() => deserializer.Deserialize<TData>(bytes)))
            .Match(
                Success<Error, TData>,
                None: () => Fail<Error, TData>(OnNone()),
                Fail: exception => Fail<Error, TData>(OnFail(exception)));

        Error OnNone()
        {
            RaiseOnDeserializeError(new DeserializeErrorEventArgs(response, DeserializeNull));
            return Error.New(DeserializeNull);
        }
        
        Error OnFail(Exception exception)
        {
            RaiseOnDeserializeError(new DeserializeErrorEventArgs(response, exception));
            return Error.New(exception);
        }
    }
}