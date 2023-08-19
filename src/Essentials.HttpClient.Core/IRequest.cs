using System.Net.Http.Headers;

namespace Essentials.HttpClient;

/// <summary>
/// Http запрос
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Название Http клиента, с помощью которого отправляется запрос
    /// </summary>
    string ClientName { get; }
    
    /// <summary>
    /// Uri запроса
    /// </summary>
    Uri Uri { get; }
    
    /// <summary>
    /// Заголовок с типом медиа
    /// </summary>
    MediaTypeHeaderValue? MediaTypeHeader { get; internal set; }
    
    /// <summary>
    /// Авторизационный заголовок
    /// </summary>
    AuthenticationHeaderValue? AuthenticationHeader { get; }
    
    /// <summary>
    /// Список заголовков запроса
    /// </summary>
    IEnumerable<(string Name, IEnumerable<string?> Values)>? Headers { get; }
    
    /// <summary>
    /// Список действий по изменению запроса
    /// </summary>
    IEnumerable<Action<HttpRequestMessage>>? ModifyRequestActions { get; }
    
    /// <summary>
    /// Таймаут запроса
    /// </summary>
    TimeSpan? Timeout { get; }
}