using System.Text;
using Essentials.HttpClient.ContentTypes.Application;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Вспомогательные методы расширения для отправки запросов с определенным типом данных в теле
/// </summary>
public static class SpecificContentTypeRequestsExtensions
{
    #region Json

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
        return await httpClient.PostStringAsync<Json>(validation, content, encoding, token);
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
        return await httpClient.PostStringAsync<Json>(request, content, encoding, token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json
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
        return await httpClient.PostDataAsync<Json, TData>(validation, data, encoding, token);
    }

    /// <summary>
    /// Отправляет Post запрос в формате Json
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
        return await httpClient.PostDataAsync<Json, TData>(request, data, encoding, token);
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
        return await httpClient.PostStringAsync<Xml>(validation, content, encoding, token);
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
        return await httpClient.PostStringAsync<Xml>(request, content, encoding, token);
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
        return await httpClient.PostDataAsync<Xml, TData>(validation, data, encoding, token);
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
        return await httpClient.PostDataAsync<Xml, TData>(request, data, encoding, token);
    }
    
    #endregion
}