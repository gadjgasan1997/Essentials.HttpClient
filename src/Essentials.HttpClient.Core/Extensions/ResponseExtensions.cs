using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization;
using Essentials.HttpClient.Errors;
using Essentials.Functional.Extensions;
using System.Diagnostics.CodeAnalysis;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для обработки полученного Http ответа
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class ResponseExtensions
{
    #region Receive Http Response Message
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static Validation<Error, HttpResponseMessage> ReceiveMessage(
        this Validation<Error, IResponse> validation)
    {
        return validation.Match(
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
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return validation.ReceiveMessage();
    }
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static HttpResponseMessage? ReceiveMessageUnsafe(
        this Validation<Error, IResponse> validation)
    {
        return validation.ReceiveMessage().MatchUnsafe(message => message, _ => null!);
    }
    
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage?> ReceiveMessageUnsafeAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return validation.ReceiveMessageUnsafe();
    }

    #endregion

    #region Receive Response String
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, string>> ReceiveStringAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .BindAsync(response => response.ReceiveStringAsync())
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, string>> ReceiveStringAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveStringAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveStringAsync()
            .MatchUnsafeAsync(@string => @string, _ => null)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает строку ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveStringUnsafeAsync().ConfigureAwait(false);
    }

    #endregion
    
    #region Receive Response Stream
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .BindAsync(response => response.ReceiveStreamAsync())
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveStreamAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveStreamAsync()
            .MatchUnsafeAsync(stream => stream, _ => null)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveStreamUnsafeAsync().ConfigureAwait(false);
    }

    #endregion

    #region Receive Response Bytes

    /// <summary>
    /// Возвращает массив байтов из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, byte[]>> ReceiveBytesAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .BindAsync(response => response.ReceiveBytesAsync())
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает массив байтов из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, byte[]>> ReceiveBytesAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveBytesAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает массив байтов из ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<byte[]?> ReceiveBytesUnsafeAsync(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveBytesAsync()
            .MatchUnsafeAsync(bytes => bytes, _ => null)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает массив байтов из ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<byte[]?> ReceiveBytesUnsafeAsync(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveBytesUnsafeAsync().ConfigureAwait(false);
    }

    #endregion

    #region Receive Response Content
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <param name="deserializerKey">Ключ десериалайзера</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(
        this Validation<Error, IResponse> validation,
        string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer
    {
        return await validation
            .BindAsync(response => response.ReceiveContentAsync<TData, TDeserializer>(deserializerKey))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <param name="deserializerKey">Ключ десериалайзера</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(
        this Task<Validation<Error, IResponse>> task,
        string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveContentAsync<TData, TDeserializer>(deserializerKey).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <param name="deserializerKey">Ключ десериалайзера</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TDeserializer>(
        this Validation<Error, IResponse> validation,
        string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer
    {
        return await validation
            .ReceiveContentAsync<TData, TDeserializer>(deserializerKey)
            .MatchUnsafeAsync(data => data, _ => default)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает десериализованное содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <param name="deserializerKey">Ключ десериалайзера</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TDeserializer>(
        this Task<Validation<Error, IResponse>> task,
        string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveContentUnsafeAsync<TData, TDeserializer>(deserializerKey).ConfigureAwait(false);
    }

    #endregion
}