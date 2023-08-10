using System.Text;
using Essentials.HttpClient.MediaTypes.Interfaces;
using Essentials.HttpClient.Serialization;
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
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TMediaType>(
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new();

    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TMediaType, TData, TSerializer>(
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer;
    
    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PutStringAsync<TMediaType>(
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new();

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IEssentialsHttpResponse>> PutDataAsync<TMediaType, TData, TSerializer>(
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer;
}