namespace Essentials.HttpClient.Models;

/// <summary>
/// Http запрос
/// </summary>
public interface IHttpRequest
{
    /// <summary>
    /// Название Http клиента, с помощью которого отправляется запрос
    /// </summary>
    string ClientName { get; }
    
    /// <summary>
    /// Сообщение запроса
    /// </summary>
    HttpRequestMessage RequestMessage { get; }
}