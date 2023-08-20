namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnSuccessSend" />
/// </summary>
public class SuccessCreateRequestEventArgs : IEventArgs
{
    internal SuccessCreateRequestEventArgs(IRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    
    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
}