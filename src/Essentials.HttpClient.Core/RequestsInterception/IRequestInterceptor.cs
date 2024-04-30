namespace Essentials.HttpClient.RequestsInterception;

/// <summary>
/// Делегат выполнения следующего действия
/// </summary>
public delegate Task<HttpResponseMessage> NextRequestDelegate();

/// <summary>
/// Интерсептор запроса
/// </summary>
public interface IRequestInterceptor
{
    /// <summary>
    /// Перехватывает запрос
    /// </summary>
    /// <param name="next">Делегат выполнения следующего действия</param>
    /// <returns>Полученный http ответ</returns>
    Task<HttpResponseMessage> Intercept(NextRequestDelegate next);
}