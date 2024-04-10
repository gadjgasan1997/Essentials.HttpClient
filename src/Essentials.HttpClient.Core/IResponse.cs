namespace Essentials.HttpClient;

/// <summary>
/// Http ответ
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
    
    /// <summary>
    /// Сообщение ответа
    /// </summary>
    HttpResponseMessage ResponseMessage { get; }
    
    /// <summary>
    /// Затраченное время на обработку запроса в милисекундах
    /// </summary>
    public long ElapsedMilliseconds { get; }
}