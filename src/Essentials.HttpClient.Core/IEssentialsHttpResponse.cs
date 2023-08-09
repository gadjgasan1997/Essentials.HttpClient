namespace Essentials.HttpClient;

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