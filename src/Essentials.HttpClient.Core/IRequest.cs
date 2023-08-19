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
    /// Сообщение запроса
    /// </summary>
    HttpRequestMessage RequestMessage { get; }
    
    /// <summary>
    /// Таймаут запроса
    /// </summary>
    TimeSpan? Timeout { get; }
}