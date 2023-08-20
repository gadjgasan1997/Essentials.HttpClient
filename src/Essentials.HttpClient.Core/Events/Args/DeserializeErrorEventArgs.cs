using Essentials.HttpClient.Events.Args.Abstractions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnSerializeError" />
/// </summary>
public class DeserializeErrorEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal DeserializeErrorEventArgs(
        IResponse response,
        Stream content,
        Exception exception)
        : base(exception, DeserializeError)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
        Content = content;
    }
    
    internal DeserializeErrorEventArgs(
        IResponse response,
        Stream content,
        string errorMessage)
        : base(new InvalidOperationException(errorMessage), errorMessage)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
        Content = content;
    }

    /// <summary>
    /// Ответ
    /// </summary>
    public IResponse Response { get; }
    
    /// <summary>
    /// Десериализуемый поток
    /// </summary>
    public Stream Content { get; }
}