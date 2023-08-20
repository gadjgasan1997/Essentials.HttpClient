namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher." />
/// </summary>
public class SuccessCreateUriEventArgs : IEventArgs
{
    internal SuccessCreateUriEventArgs(Uri uri)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
    }
    
    /// <summary>
    /// Адрес запроса
    /// </summary>
    public Uri Uri { get; }
}