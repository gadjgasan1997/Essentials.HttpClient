using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.HttpClient.MediaTypes;
using Essentials.HttpClient.Serialization.Implementations;
using LanguageExt;
using LanguageExt.Common;
using TextXml = Essentials.HttpClient.MediaTypes.Text.Xml;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для отправки Http запросов с типами содерждимого из <see cref="Storage.Text" />
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
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostTextXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(validation, content, new TextXml(), encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostTextXmlStringAsync(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostStringAsync(request, content, new TextXml(), encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostTextXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, XmlSerializer>(validation, data, new TextXml(), encoding, token);
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
    public static async Task<Validation<Error, IEssentialsHttpResponse>> PostTextXmlDataAsync<TData>(
        this IEssentialsHttpClient httpClient,
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await httpClient.PostDataAsync<TData, XmlSerializer>(request, data, new TextXml(), encoding, token);
    }

    #endregion
}