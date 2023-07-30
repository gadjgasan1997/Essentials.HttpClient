namespace Essentials.HttpClient.Models;

/// <summary>
/// Http ответ
/// </summary>
public interface IHttpResponse
{
    /// <summary>
    /// Сообщение ответа
    /// </summary>
    HttpResponseMessage ResponseMessage { get; }
}