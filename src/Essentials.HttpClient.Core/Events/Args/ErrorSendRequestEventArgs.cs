using Essentials.HttpClient.Events.Args.Abstractions;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnErrorSend" />
/// </summary>
public class ErrorSendRequestEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal ErrorSendRequestEventArgs(
        IRequest request,
        Exception exception,
        string? errorMessage = null)
        : base(exception, errorMessage ?? "Во время отправки запроса произошло исключение")
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
}