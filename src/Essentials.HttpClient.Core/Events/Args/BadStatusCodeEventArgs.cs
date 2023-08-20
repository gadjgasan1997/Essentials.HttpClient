using System.Net;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnBadStatusCode" />
/// </summary>
public class BadStatusCodeEventArgs : IEventArgs
{
    internal BadStatusCodeEventArgs(
        HttpStatusCode errorCode,
        IRequest request,
        HttpResponseMessage responseMessage)
    {
        ErrorCode = errorCode;
        Request = request ?? throw new ArgumentNullException(nameof(request));
        ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
    }
    
    /// <summary>
    /// Код ошибки
    /// </summary>
    public HttpStatusCode ErrorCode { get; }

    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
    
    /// <summary>
    /// Сообщение об ответе
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; }
}