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
[SuppressMessage("ReSharper", "UnusedMember.Global")]
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
    public static async Task<Validation<Error, IResponse>> GetAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.BindAsync(request => httpClient.GetAsync(request, token)).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="uri">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> GetAsync(
        this IEssentialsHttpClient httpClient,
        Uri uri,
        CancellationToken? token = null)
    {
        var uriValidation = await UriBuilderFactory
            .CreateBuilder(uri)
            .BuildAsync()
            .ConfigureAwait(false);

        return await RequestBuilderFactory
            .CreateBuilder(uriValidation)
            .BuildAsync()
            .BindAsync(request => httpClient.GetAsync(request, token))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="address">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> GetAsync(
        this IEssentialsHttpClient httpClient,
        string address,
        CancellationToken? token = null)
    {
        return await httpClient.GetAsync(new Uri(address), token).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу и возвращает строку ответа
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="uri">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<string?> GetStringAsync(
        this IEssentialsHttpClient httpClient,
        Uri uri,
        CancellationToken? token = null)
    {
        return await httpClient
            .GetAsync(uri, token)
            .ReceiveStringUnsafeAsync()
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу и возвращает строку ответа
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="address">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<string?> GetStringAsync(
        this IEssentialsHttpClient httpClient,
        string address,
        CancellationToken? token = null)
    {
        return await httpClient.GetStringAsync(new Uri(address), token).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу и возвращает поток с ответом
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="uri">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Stream?> GetStreamAsync(
        this IEssentialsHttpClient httpClient,
        Uri uri,
        CancellationToken? token = null)
    {
        return await httpClient
            .GetAsync(uri, token)
            .ReceiveStreamUnsafeAsync()
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Get запрос по указанному адресу и возвращает поток с ответом
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="address">Адрес запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Stream?> GetStreamAsync(
        this IEssentialsHttpClient httpClient,
        string address,
        CancellationToken? token = null)
    {
        return await httpClient.GetStreamAsync(new Uri(address), token).ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> HeadAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.BindAsync(request => httpClient.HeadAsync(request, token)).ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient
            .PostAsync(request, new StringContent(content), mediaType, encoding, token)
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(request =>
                httpClient.PostStringAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        Stream content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient
            .PostAsync(request, new StreamContent(content), mediaType, encoding, token)
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        Stream content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(request =>
                httpClient.PostStreamAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(data)
            .BindAsync(content => httpClient.PostAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(request =>
                httpClient.PostDataAsync<TData, TSerializer>(request, data, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient
            .PutAsync(request, new StringContent(content), mediaType, encoding, token)
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(request =>
                httpClient.PutStringAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(data)
            .BindAsync(content => httpClient.PutAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(request =>
                httpClient.PutDataAsync<TData, TSerializer>(request, data, mediaType, encoding, token))
            .ConfigureAwait(false);
    }
    
    #endregion

    #region Patch Requests

    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient
            .PatchAsync(request, new StringContent(content), mediaType, encoding, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(request =>
                httpClient.PatchStringAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
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
    public static async Task<Validation<Error, IResponse>> PatchDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(data)
            .BindAsync(content => httpClient.PatchAsync(request, content, mediaType, encoding, token))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
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
    public static async Task<Validation<Error, IResponse>> PatchDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(request =>
                httpClient.PatchDataAsync<TData, TSerializer>(request, data, mediaType, encoding, token))
            .ConfigureAwait(false);
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
    public static async Task<Validation<Error, IResponse>> DeleteAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        CancellationToken? token = default)
    {
        return await validation
            .BindAsync(request => httpClient.DeleteAsync(request, token))
            .ConfigureAwait(false);
    }

    #endregion

    /// <summary>
    /// Создает содержимое запроса с потоком из данных
    /// </summary>
    /// <param name="data">Данные</param>
    /// <typeparam name="TData">Тип данных</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private static Validation<Error, StreamContent> BuildStreamContent<TData, TSerializer>(TData data)
    {
        return SerializersCreator
            .GetSerializer<TSerializer>()
            .Bind(serializer => serializer.SerializeObject(data))
            .Bind<StreamContent>(stream => new StreamContent(stream));
    }
}