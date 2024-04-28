using System.Diagnostics.CodeAnalysis;
using Essentials.Functional.Extensions;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization;

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
        return await validation
            .BindAsync(async request =>
                await httpClient.GetAsync(request, token).ConfigureAwait(false))
            .ConfigureAwait(false);
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
        return await HttpRequestBuilder
            .CreateBuilder(uri)
            .BuildAsync()
            .BindAsync(async request =>
                await httpClient.GetAsync(request, token).ConfigureAwait(false))
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
        return await HttpUriBuilder
            .CreateBuilder(address)
            .BuildAsync()
            .MatchUnsafeAsync(
                async _ => await httpClient.GetStringAsync(new Uri(address), token).ConfigureAwait(false),
                _ => (string?) null)
            .ConfigureAwait(false);
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
        return await validation
            .BindAsync(async request =>
                await httpClient.HeadAsync(request, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    #endregion

    #region Post Requests

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        return await httpClient
            .PostAsync(request, new StringContent(content), token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostStringAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        Stream content,
        CancellationToken? token = null)
    {
        return await httpClient
            .PostAsync(request, new StreamContent(content), token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Поток с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostStreamAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        Stream content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostStreamAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(request, data)
            .BindAsync(async content =>
                await httpClient.PostAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostDataAsync<TData, TSerializer>(request, data, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="serializerKey">Ключ сериалайзера</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        string serializerKey,
        IRequest request,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(request, data, serializerKey)
            .BindAsync(async content =>
                await httpClient.PostAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="serializerKey">Ключ сериалайзера</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        string serializerKey,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostDataAsync<TData, TSerializer>(serializerKey, request, data, token)
                    .ConfigureAwait(false))
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
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        return await httpClient
            .PutAsync(request, new StringContent(content), token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PutStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PutStringAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(request, data)
            .BindAsync(async content =>
                await httpClient.PutAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PutDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PutDataAsync<TData, TSerializer>(request, data, token).ConfigureAwait(false))
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
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        return await httpClient
            .PatchAsync(request, new StringContent(content), token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PatchStringAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await BuildStreamContent<TData, TSerializer>(request, data)
            .BindAsync(async content =>
                await httpClient.PatchAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PatchDataAsync<TData, TSerializer>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
        where TSerializer : IEssentialsSerializer
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PatchDataAsync<TData, TSerializer>(request, data, token).ConfigureAwait(false))
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
            .BindAsync(async request =>
                await httpClient.DeleteAsync(request, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    #endregion

    #region Private
    
    /// <summary>
    /// Создает содержимое запроса с потоком из данных
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="data">Данные</param>
    /// <typeparam name="TData">Тип данных</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private static Validation<Error, StreamContent> BuildStreamContent<TData, TSerializer>(
        IRequest request,
        TData data)
        where TSerializer : IEssentialsSerializer
    {
        return SerializersManager
            .GetSerializer<TSerializer>()
            .Bind(serializer => BuildStreamContent(serializer, request, data));
    }

    /// <summary>
    /// Создает содержимое запроса с потоком из данных
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="data">Данные</param>
    /// <param name="serializerKey">Ключ сериалайзера</param>
    /// <typeparam name="TData">Тип данных</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private static Validation<Error, StreamContent> BuildStreamContent<TData, TSerializer>(
        IRequest request,
        TData data,
        string serializerKey)
        where TSerializer : IEssentialsSerializer
    {
        return SerializersManager
            .GetSerializer<TSerializer>(serializerKey)
            .Bind(serializer => BuildStreamContent(serializer, request, data));
    }
    
    /// <summary>
    /// Создает содержимое запроса с потоком из данных
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <param name="request">Запрос</param>
    /// <param name="data">Данные</param>
    /// <typeparam name="TData">Тип данных</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private static Validation<Error, StreamContent> BuildStreamContent<TData, TSerializer>(
        TSerializer serializer,
        IRequest request,
        TData data)
        where TSerializer : IEssentialsSerializer
    {
        return serializer.SerializeObject(request, data)
            .Bind<MemoryStream>(bytes => new MemoryStream(bytes))
            .Bind<StreamContent>(stream => new StreamContent(stream));
    }

    #endregion
}