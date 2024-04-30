using System.Diagnostics.Contracts;
using Essentials.HttpClient.Extensions;
using LanguageExt;
using LanguageExt.Common;
using Essentials.Utils.IO.Extensions;
using Essentials.Serialization;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Events.EventsStorage;
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
    public static Validation<Error, byte[]> SerializeObject<T>(
        this IEssentialsSerializer serializer,
        IRequest request,
        T content)
    {
        using var scope = HttpRequestContext.CreateContext(request);

        return TryOption(() => serializer.Serialize(content))
            .Match(
                Success<Error, byte[]>,
                None: () => OnNone(),
                Fail: exception => OnFail(exception));

        Error OnNone()
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(new InvalidOperationException(SerializeNull), SerializeNull);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnSerializeError), RaiseOnSerializeError);
            
            return Error.New(SerializeNull);
        }

        Error OnFail(Exception exception)
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(exception, SerializeError);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnSerializeError), RaiseOnSerializeError);
            
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
        using var scope = HttpRequestContext.RestoreContext(response);

        return Try(stream.AsMemory())
            .Map(Handle)
            .Match(
                Succ: validation => validation,
                Fail: exception => Fail<Error, TData>(OnFail(exception)));

        Validation<Error, TData> Handle(Memory<byte> memory)
        {
            return TryOption(() => deserializer.Deserialize<TData>(memory))
                .Match(
                    Some: data => Success<Error, TData>(data!),
                    None: () => Fail<Error, TData>(OnNone()),
                    Fail: exception => Fail<Error, TData>(OnFail(exception)));
        }
        
        Error OnNone()
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(new InvalidOperationException(DeserializeNull), DeserializeNull);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnDeserializeError), RaiseOnDeserializeError);
            
            return Error.New(DeserializeNull);
        }
        
        Error OnFail(Exception exception)
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(exception, DeserializeError);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnDeserializeError), RaiseOnDeserializeError);
            
            return Error.New(exception);
        }
    }
}