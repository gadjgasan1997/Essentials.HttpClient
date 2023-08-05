namespace Essentials.HttpClient.Models;

/// <summary>
/// Http ответ
/// </summary>
public interface IEssentialsHttpResponse
{
    /// <summary>
    /// Сообщение ответа
    /// </summary>
    HttpResponseMessage ResponseMessage { get; }
}