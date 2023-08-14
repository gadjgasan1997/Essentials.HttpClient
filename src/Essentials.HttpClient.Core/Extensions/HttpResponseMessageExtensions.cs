using Essentials.Func.Utils.Extensions;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="HttpResponseMessage" />
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Возвращает строку с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static Task<Validation<Error, string>> ReceiveStringAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return TryOptionAsync(() => message.Content.ReadAsStringAsync())
            .Match(
                Some: Validation<Error, string>.Success,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Содержимое ответа равно null"));
    }
    
    /// <summary>
    /// Возвращает строку с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static Task<string?> ReceiveStringUnsafeAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return message
            .ReceiveStringAsync()
            .DefaultMatchUnsafeAsync(str => str, _ => null);
    }
    
    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static Task<Validation<Error, Stream>> ReceiveStreamAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return TryOptionAsync(() => message.Content.ReadAsStreamAsync())
            .Match(
                Some: Validation<Error, Stream>.Success,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Содержимое ответа равно null"));
    }
    
    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static Task<Stream?> ReceiveStreamUnsafeAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return message
            .ReceiveStreamAsync()
            .DefaultMatchUnsafeAsync(stream => stream, _ => null);
    }
}