namespace Essentials.HttpClient.Events.Args.Abstractions;

/// <summary>
/// Базовый класс для аргументов ошибочных событий
/// </summary>
public abstract class BaseErrorEventArgs
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="exception">Возникшее исключение</param>
    /// <param name="errorMessage">Сообщение об ошибке</param>
    protected BaseErrorEventArgs(Exception exception, string errorMessage)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }

    /// <summary>
    /// Возникшее исключение
    /// </summary>
    public Exception Exception { get; }
    
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; }
}