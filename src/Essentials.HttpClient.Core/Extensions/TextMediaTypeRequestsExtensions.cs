using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Serialization.Implementations;
using LanguageExt;
using LanguageExt.Common;
using static Essentials.HttpClient.Helpers.HttpRequestsHelpers;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для отправки Http запросов
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class TextMediaTypeRequestsExtensions
{
    #region Xml

    /// <summary>
    /// Отправляет Post запрос в формате Xml
    /// </summary>
    /// <param name="httpClient">Http клиента</param>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IResponse>> PostTextXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        string content,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostTextXmlStringAsync(request, content, token).ConfigureAwait(false))
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
    public static async Task<Validation<Error, IResponse>> PostTextXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        string content,
        CancellationToken? token = null)
    {
        request.MediaTypeHeader = GetTextXmlMediaTypeHeader(request.MediaTypeHeader?.CharSet);
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
    public static async Task<Validation<Error, IResponse>> PostTextXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IRequest> validation,
        TData data,
        CancellationToken? token = null)
    {
        return await validation
            .BindAsync(async request =>
                await httpClient.PostTextXmlDataAsync(request, data, token).ConfigureAwait(false))
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
    public static async Task<Validation<Error, IResponse>> PostTextXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IRequest request,
        TData data,
        CancellationToken? token = null)
    {
        request.MediaTypeHeader = GetTextXmlMediaTypeHeader(request.MediaTypeHeader?.CharSet);
        
        return await httpClient
            .PostDataAsync<TData, XmlSerializer>(request, data, token)
            .ConfigureAwait(false);
    }

    #endregion
}