using Essentials.HttpClient.Events.Args.Abstractions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Events.Args;

/// <summary>
/// Аргументы события <see cref="EventsPublisher.OnSerializeError" />
/// </summary>
public class SerializeErrorEventArgs : BaseErrorEventArgs, IEventArgs
{
    internal SerializeErrorEventArgs(
        IRequest request,
        object? content,
        Exception exception)
        : base(exception, SerializeError)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Content = content;
    }
    
    internal SerializeErrorEventArgs(
        IRequest request,
        object? content,
        string errorMessage)
        : base(new InvalidOperationException(errorMessage), errorMessage)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Content = content;
    }
    
    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
    
    /// <summary>
    /// Содержимое
    /// </summary>
    public object? Content { get; }
}