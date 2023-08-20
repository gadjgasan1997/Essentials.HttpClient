using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Events.Args;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;
using static Essentials.HttpClient.Events.EventsPublisher;

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
    public static async Task<Validation<Error, string>> ReceiveStringAsync(this HttpResponseMessage message)
    {
        return await TryOptionAsync(() => message.Content.ReadAsStringAsync())
            .Match(
                Some: Validation<Error, string>.Success,
                None: () => OnNone(message),
                Fail: exception => OnFail(message, exception))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает строку с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(this HttpResponseMessage message)
    {
        return await message
            .ReceiveStringAsync()
            .MatchUnsafeAsync(@string => @string, _ => null)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(this HttpResponseMessage message)
    {
        return await TryOptionAsync(() => message.Content.ReadAsStreamAsync())
            .Match(
                Some: Validation<Error, Stream>.Success,
                None: () => OnNone(message),
                Fail: exception => OnFail(message, exception))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(this HttpResponseMessage message)
    {
        return await message
            .ReceiveStreamAsync()
            .MatchUnsafeAsync(stream => stream, _ => null)
            .ConfigureAwait(false);
    }
    
    private static Error OnNone(HttpResponseMessage message)
    {
        RaiseOnErrorReadContent(new ErrorReadContentEventArgs(message, ContentIsNull));
        return Error.New(ContentIsNull);   
    }
        
    private static Error OnFail(HttpResponseMessage message, Exception exception)
    {
        RaiseOnErrorReadContent(new ErrorReadContentEventArgs(message, exception));
        return Error.New(exception);
    }
}