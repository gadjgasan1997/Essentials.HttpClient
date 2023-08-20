namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnSuccessCreateRequest" />
/// </summary>
public class SuccessSendRequestEventArgs : IEventArgs
{
    internal SuccessSendRequestEventArgs(IRequest request, IResponse response)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }

    /// <summary>
    /// Ответ
    /// </summary>
    public IResponse Response { get; }
}