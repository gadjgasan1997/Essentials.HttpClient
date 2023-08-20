using Essentials.HttpClient.Events.Args.Abstractions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnErrorReadContent" />
/// </summary>
public class ErrorReadContentEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal ErrorReadContentEventArgs(
        HttpResponseMessage responseMessage,
        Exception exception)
        : base(exception, string.Format(ErrorGetContent, exception.Message))
    {
        ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
    }
    
    internal ErrorReadContentEventArgs(
        HttpResponseMessage responseMessage,
        string errorMessage)
        : base(new InvalidOperationException(errorMessage), errorMessage)
    {
        ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
    }
    
    /// <summary>
    /// Сообщение об ответе
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; }
}