using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.MediaTypes.Interfaces;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширений для Http клиентов
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class HttpClientsExtensions
{
    #region Get Requests

    /// <summary>
    /// Отправляет Get запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.DefaultBindAsync(request => httpClient.GetAsync(request, token));
    }

    #endregion

    #region Head Requests

    /// <summary>
    /// Отправляет Head запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> HeadAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.DefaultBindAsync(request => httpClient.HeadAsync(request, token));
    }

    #endregion

    #region Post Requests

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostAsync(request, new StringContent(content), mediaType, encoding, token);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PostStringAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        Stream content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostAsync(request, new StreamContent(content), mediaType, encoding, token);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        Stream content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PostStreamAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(data)
            .DefaultBindAsync(content => httpClient.PostAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PostDataAsync<TData, TSerializer>(request, data, mediaType, encoding, token));
    }
    
    #endregion

    #region Put Requests

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PutAsync(request, new StringContent(content), mediaType, encoding, token);
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PutStringAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutStreamAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        StreamContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PutAsync(request, content, mediaType, encoding, token);
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutStreamAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        StreamContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PutStreamAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(data)
            .DefaultBindAsync(content => httpClient.PutAsync(request, content, mediaType, encoding, token));
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation.DefaultBindAsync(request =>
            httpClient.PutDataAsync<TData, TSerializer>(request, data, mediaType, encoding, token));
    }
    
    #endregion

    #region Delete Requests
    
    /// <summary>
    /// Отправляет Delete запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> DeleteAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.DefaultBindAsync(request => httpClient.DeleteAsync(request, token));
    }

    #endregion

    /// <summary>
    /// Создает содержимое запроса с потоком из данных
    /// </summary>
    /// <param name="data">Данные</param>
    /// <typeparam name="TData">Тип данных</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private static async Task<Validation<Error, StreamContent>> BuildStreamContent<TData, TSerializer>(TData data)
    {
        return await SerializersCreator
            .GetSerializer<TSerializer>()
            .DefaultBindAsync(serializer => serializer.SerializeObject(data))
            .DefaultBindAsync(stream => new StreamContent(stream));
    }
}