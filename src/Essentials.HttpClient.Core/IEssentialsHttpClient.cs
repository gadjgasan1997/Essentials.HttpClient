using LanguageExt;
using LanguageExt.Common;
using Token = System.Threading.CancellationToken;

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
    Task<Validation<Error, IResponse>> GetAsync(IRequest request, Token? token = null);
    
    /// <summary>
    /// Отправляет Head запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IResponse>> HeadAsync(IRequest request, Token? token = null);
    
    /// <summary>
    /// Отправляет Post запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IResponse>> PostAsync(IRequest request, HttpContent content, Token? token = null);

    /// <summary>
    /// Отправляет Put запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IResponse>> PutAsync(IRequest request, HttpContent content, Token? token = null);
    
    /// <summary>
    /// Отправляет Patch запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IResponse>> PatchAsync(IRequest request, HttpContent content, Token? token = null);
    
    /// <summary>
    /// Отправляет Delete запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    Task<Validation<Error, IResponse>> DeleteAsync(IRequest request, Token? token = null);
}