using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Errors;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для обработки полученного Http ответа
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class ResponseExtensions
{
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, HttpResponseMessage>> ReceiveMessageAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.DefaultMatchAsync(
            Succ: response => response.ResponseMessage,
            Fail: seq => seq.OfType<BadStatusCodeError>().FirstOrDefault() is { } error
                ? error.ResponseMessage
                : Fail<Error, HttpResponseMessage>(seq));
    }
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, HttpResponseMessage>> ReceiveMessageAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveMessageAsync();
    }
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage?> ReceiveMessageUnsafeAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation
            .ReceiveMessageAsync()
            .DefaultMatchUnsafeAsync(message => message, _ => default);
    }
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage?> ReceiveMessageUnsafeAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveMessageUnsafeAsync();
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, string>> ReceiveStringAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.DefaultBindAsync(response => response.ResponseMessage.ReceiveStringAsync());
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, string>> ReceiveStringAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveStringAsync();
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation
            .ReceiveStringAsync()
            .DefaultMatchUnsafeAsync(@string => @string, _ => null);
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveStringUnsafeAsync();
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.DefaultBindAsync(response => response.ResponseMessage.ReceiveStreamAsync());
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveStreamAsync();
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation
            .ReceiveStreamAsync()
            .DefaultMatchUnsafeAsync(stream => stream, _ => null);
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveStreamUnsafeAsync();
    }
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(
        this Validation<Error, IEssentialsHttpResponse> validation)
        where TDeserializer : IEssentialsDeserializer
    {
        return await validation.DefaultBindAsync(DeserializeResponseAsync<TData, TDeserializer>);
    }

    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
        where TDeserializer : IEssentialsDeserializer
    {
        var validation = await task;
        return await validation.ReceiveContentAsync<TData, TDeserializer>();
    }
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TDeserializer>(
        this Validation<Error, IEssentialsHttpResponse> validation)
        where TDeserializer : IEssentialsDeserializer
    {
        return await validation
            .ReceiveContentAsync<TData, TDeserializer>()
            .DefaultMatchUnsafeAsync(data => data, _ => default);
    }
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TDeserializer>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
        where TDeserializer : IEssentialsDeserializer
    {
        var validation = await task;
        return await validation.ReceiveContentUnsafeAsync<TData, TDeserializer>();
    }
    
    /// <summary>
    /// Десерилизует ответ в объект
    /// </summary>
    /// <param name="response">Ответ</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    internal static async Task<Validation<Error, TData>> DeserializeResponseAsync<TData, TDeserializer>(
        this IEssentialsHttpResponse response)
        where TDeserializer : IEssentialsDeserializer
    {
        return await (
                response.ResponseMessage.ReceiveStreamAsync().Result,
                SerializersCreator.GetDeserializer<TDeserializer>())
            .Apply((stream, deserializer) => deserializer.DeserializeStream<TData>(stream))
            .DefaultBindAsync(task => task);
    }
}