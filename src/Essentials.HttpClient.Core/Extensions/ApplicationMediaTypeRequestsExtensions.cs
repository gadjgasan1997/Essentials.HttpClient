using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.HttpClient.MediaTypes;
using Essentials.HttpClient.Serialization.Implementations;
using LanguageExt;
using LanguageExt.Common;
using ApplicationJson= Essentials.HttpClient.MediaTypes.Application.Json;
using ApplicationXml = Essentials.HttpClient.MediaTypes.Application.Xml;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для отправки Http запросов с типами содерждимого из <see cref="Storage.Application" />
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class ApplicationMediaTypeRequestsExtensions
{
    /// <summary>
    /// Отправляет Post запрос в формате Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(
            validation,
            content,
            new ApplicationJson(),
            encoding,
            token);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(
            request,
            content,
            new ApplicationJson(),
            encoding,
            token);
    }
    
    #region Newtonsoft Json
    
    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, NewtonsoftJsonSerializer>(
            validation,
            data,
            new ApplicationJson(),
            encoding,
            token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, NewtonsoftJsonSerializer>(
            request,
            data,
            new ApplicationJson(),
            encoding,
            token);
    }
    
    #endregion
    
    #region Native Json

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostNativeJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, NativeJsonSerializer>(
            validation,
            data,
            new ApplicationJson(),
            encoding,
            token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostNativeJsonDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, NativeJsonSerializer>(
            request,
            data,
            new ApplicationJson(),
            encoding,
            token);
    }
    
    #endregion
    
    #region Xml
    
    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostApplicationXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(
            validation,
            content,
            new ApplicationXml(),
            encoding,
            token);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostApplicationXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(
            request,
            content,
            new ApplicationXml(),
            encoding,
            token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostApplicationXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, XmlSerializer>(
            validation,
            data,
            new ApplicationXml(),
            encoding,
            token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostApplicationXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, XmlSerializer>(
            request,
            data,
            new ApplicationXml(),
            encoding,
            token);
    }
    
    #endregion
}