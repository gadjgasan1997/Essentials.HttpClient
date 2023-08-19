namespace Essentials.HttpClient;

/// <summary>
/// Http ответ
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Сообщение ответа
    /// </summary>
    HttpResponseMessage ResponseMessage { get; }
}