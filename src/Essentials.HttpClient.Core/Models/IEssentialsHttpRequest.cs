using System.Text;

namespace Essentials.HttpClient.Models;

/// <summary>
/// Http запрос
/// </summary>
public interface IEssentialsHttpRequest
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
    
    /// <summary>
    /// Кодировка содержимого запроса
    /// </summary>
    Encoding Encoding { get; }
}