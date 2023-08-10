using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.HttpClient.MediaTypes.Application;
using Essentials.HttpClient.Serialization.Implementations;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для отправки Http запросов с определенным типом данных в теле
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class SpecificMediaTypeRequestsExtensions
{
    #region Newtonsoft Json

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
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
        return await httpClient.PostStringAsync<Json, NewtonsoftJsonSerializer>(validation, content, encoding, token);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием Newtonsoft.Json
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
        return await httpClient.PostStringAsync<Json, NewtonsoftJsonSerializer>(request, content, encoding, token);
    }

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
        return await httpClient.PostDataAsync<Json, TData, NewtonsoftJsonSerializer>(validation, data, encoding, token);
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
        return await httpClient.PostDataAsync<Json, TData, NewtonsoftJsonSerializer>(request, data, encoding, token);
    }
    
    #endregion
    
    #region Native Json

    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostNativeJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync<Json, NativeJsonSerializer>(validation, content, encoding, token);
    }
    
    /// <summary>
    /// Отправляет Post запрос в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostNativeJsonStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync<Json, NativeJsonSerializer>(request, content, encoding, token);
    }

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
        return await httpClient.PostDataAsync<Json, TData, NativeJsonSerializer>(validation, data, encoding, token);
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
        return await httpClient.PostDataAsync<Json, TData, NativeJsonSerializer>(request, data, encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync<Xml, XmlSerializer>(validation, content, encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync<Xml, XmlSerializer>(request, content, encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<Xml, TData, XmlSerializer>(validation, data, encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<Xml, TData, XmlSerializer>(request, data, encoding, token);
    }
    
    #endregion
}