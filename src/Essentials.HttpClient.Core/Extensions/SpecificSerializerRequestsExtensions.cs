using System.Diagnostics.CodeAnalysis;
using Essentials.Functional.Extensions;
using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization.Serializers;
using static Essentials.HttpClient.Helpers.HttpRequestsHelpers;
using static Essentials.HttpClient.Dictionaries.KnownHttpClientSerializers;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для отправки Http запросов с определенным типом сериалайзера
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class SpecificSerializerRequestsExtensions
{
    /// <summary>
    /// Отправляет Post запрос в формате Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostJsonStringAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        request.IfNone(GetJsonMediaTypeHeader());
        return await httpClient.PostStringAsync(request, content, token).ConfigureAwait(false);
    }
    
    #region Newtonsoft Json
    
    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostJsonDataAsync(request, data, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
    {
        request.IfNone(GetJsonMediaTypeHeader());
        
        return await httpClient
            .PostDataAsync<TData, NewtonsoftJsonSerializer>(NEWTONSOFT_JSON, request, data, token)
            .ConfigureAwait(false);
    }
    
    #endregion
    
    #region Native Json

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostNativeJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostNativeJsonDataAsync(request, data, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostNativeJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
    {
        request.IfNone(GetJsonMediaTypeHeader());

        return await httpClient
            .PostDataAsync<TData, NativeJsonSerializer>(NATIVE_JSON, request, data, token)
            .ConfigureAwait(false);
    }
    
    #endregion
    
    #region Xml
    
    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostXmlStringAsync(request, content, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        request.IfNone(GetXmlMediaTypeHeader());
        return await httpClient.PostStringAsync(request, content, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostXmlDataAsync(request, data, token).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
    {
        request.IfNone(GetXmlMediaTypeHeader());

        return await httpClient
            .PostDataAsync<TData, XmlSerializer>(XML, request, data, token)
            .ConfigureAwait(false);
    }
    
    #endregion
}