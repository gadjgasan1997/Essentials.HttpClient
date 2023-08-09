using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.ContentTypes.Interfaces;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IEssentialsHttpResponse" />
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class EssentialsHttpResponseExtensions
{
    /// <summary>
    /// Возвращает сообщение ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <returns></returns>
    public static async Task<Validation<Error, HttpResponseMessage>> ReceiveMessageAsync(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.DefaultBindAsync(response => response.ResponseMessage);
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
    /// Возвращает содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TContentType"></typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TContentType>(
        this Validation<Error, IEssentialsHttpResponse> validation)
        where TContentType : IContentType, new()
    {
        return await validation.DefaultBindAsync(response =>
            DeserializeResponseAsync<TData, TContentType>(response, new TContentType()));
    }

    /// <summary>
    /// Возвращает содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TContentType"></typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TContentType>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
        where TContentType : IContentType, new()
    {
        var validation = await task;
        return await validation.ReceiveContentAsync<TData, TContentType>();
    }
    
    /// <summary>
    /// Возвращает содержимое ответа
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TContentType"></typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TContentType>(
        this Validation<Error, IEssentialsHttpResponse> validation)
        where TContentType : IContentType, new()
    {
        return await validation
            .ReceiveContentAsync<TData, TContentType>()
            .DefaultMatchUnsafeAsync(data => data, _ => default);
    }
    
    /// <summary>
    /// Возвращает содержимое ответа
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TContentType"></typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveContentUnsafeAsync<TData, TContentType>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
        where TContentType : IContentType, new()
    {
        var validation = await task;
        return await validation.ReceiveContentUnsafeAsync<TData, TContentType>();
    }
    
    /// <summary>
    /// Десерилизует ответ в объект
    /// </summary>
    /// <param name="response">Ответ</param>
    /// <param name="contentType">Тип содержимого</param>
    /// <typeparam name="TData">Тип объекта</typeparam>
    /// <typeparam name="TContentType">Тип содержимого</typeparam>
    /// <returns></returns>
    private static async Task<Validation<Error, TData>> DeserializeResponseAsync<TData, TContentType>(
        IEssentialsHttpResponse response,
        TContentType contentType)
    where TContentType : IContentType
    {
        return await (
                response.ResponseMessage.ReceiveStringAsync().Result,
                SerializersCreator.GetDeserializer(contentType))
            .Apply((content, deserializer) => deserializer.DeserializeString<TData>(content))
            .DefaultBindAsync(task => task);
    }
}