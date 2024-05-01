using System.Net.Sockets;
using Essentials.HttpClient.Dictionaries;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для исключений
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Возвращает сообщение об ошибке выполнения http запроса
    /// </summary>
    /// <param name="exception">Возникшее исключение</param>
    /// <param name="request">Запрос</param>
    /// <returns>Сообщение об ошибке</returns>
    public static string ToHttpRequestExceptionMessage(this Exception exception, IRequest request) =>
        exception switch
        {
            TimeoutException or { InnerException: TimeoutException } =>
                string.Format(TimeoutError, request.Uri, exception.Message),
            
            TaskCanceledException or { InnerException: TaskCanceledException } =>
                string.Format(TaskCanceledError, request.Uri, exception.Message),
            
            SocketException or { InnerException: SocketException } =>
                string.Format(ErrorMessages.SocketError, request.Uri, exception.Message),
            
            _ => exception.Message
        };
}