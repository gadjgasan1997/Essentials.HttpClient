using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Errors;

/// <summary>
/// Ошибка неверного Http кода ответа
/// </summary>
public record BadStatusCodeError : Expected
{
    private BadStatusCodeError(
        HttpResponseMessage responseMessage,
        string Message,
        Option<Error> Inner = new())
        : base(Message, (int) responseMessage.StatusCode, Inner)
    {
        ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
    }
    
    /// <summary>
    /// Сообщение об ответе
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; }

    /// <summary>
    /// Создает экземпляр ошибки
    /// </summary>
    /// <param name="responseMessage">Полученный Http ответ</param>
    /// <param name="errorMessage">Сообщение об ошибке</param>
    /// <returns></returns>
    public static BadStatusCodeError New(HttpResponseMessage responseMessage, string errorMessage) =>
        new(responseMessage, errorMessage);
}