using Essentials.HttpClient.Events.Args.Abstractions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnErrorCreateRequest" />
/// </summary>
public class ErrorCreateRequestEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal ErrorCreateRequestEventArgs(
        Uri? uri,
        Exception exception,
        string? errorMessage = null)
        : base(exception, errorMessage ?? string.Format(ErrorCreateRequest, exception.Message))
    {
        Uri = uri;
    }
    
    /// <summary>
    /// Адрес запроса
    /// </summary>
    public Uri? Uri { get; }
}