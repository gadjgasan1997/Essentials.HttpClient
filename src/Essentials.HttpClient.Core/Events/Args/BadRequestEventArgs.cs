using Essentials.HttpClient.Events.Args.Abstractions;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnBadRequest" />
/// </summary>
public class BadRequestEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal BadRequestEventArgs(string errorMessage)
        : base(new ArgumentException(errorMessage), errorMessage)
    { }
}