using System.Text;
using Essentials.HttpClient.MediaTypes.Interfaces;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

/// <summary>
/// Http клиент
/// </summary>
public interface IEssentialsHttpClient
{
    /// <summary>
    /// Отправляет Get запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = null);
    
    /// <summary>
    /// Отправляет Head запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> HeadAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = null);
    
    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostAsync(
        IEssentialsHttpRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null);
    
    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PutAsync(
        IEssentialsHttpRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null);
}