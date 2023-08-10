using System.Text;
using Essentials.HttpClient.ContentTypes.Interfaces;
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
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default);
    
    /// <summary>
    /// Отправляет Get запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = default);

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TContentType">Тип контента содержимого (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new();

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TContentType">Тип контента содержимого (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new();

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="validation">Объект Validation с Http запросом</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TContentType">Тип контента содержимого (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TData>(
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new();

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TContentType">Тип контента содержимого (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TData>(
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new();
}