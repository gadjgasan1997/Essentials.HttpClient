using Essentials.HttpClient.Events.Args.Abstractions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnErrorCreateUri" />
/// </summary>
public class ErrorCreateUriEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal ErrorCreateUriEventArgs(
        string? address,
        Exception exception,
        string? errorMessage = null)
        : base(exception, errorMessage ?? string.Format(ErrorCreateUri, exception.Message))
    {
        Address = address;
    }
    
    /// <summary>
    /// Адрес запроса
    /// </summary>
    public string? Address { get; }
}