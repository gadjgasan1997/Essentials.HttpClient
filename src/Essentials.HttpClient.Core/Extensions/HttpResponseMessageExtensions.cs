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
    public static async Task<Validation<Error, string>> ReceiveStringAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return await TryOptionAsync(() => message.Content.ReadAsStringAsync())
            .Match(
                Some: Validation<Error, string>.Success,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Содержимое ответа равно null"))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает строку с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static async Task<string?> ReceiveStringUnsafeAsync(this HttpResponseMessage message)
    {
        // TODO Log
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
        // TODO Log
        return await TryOptionAsync(() => message.Content.ReadAsStreamAsync())
            .Match(
                Some: Validation<Error, Stream>.Success,
                Fail: exception => Error.New(exception),
                None: () => Error.New("Содержимое ответа равно null"))
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <returns></returns>
    public static async Task<Stream?> ReceiveStreamUnsafeAsync(this HttpResponseMessage message)
    {
        // TODO Log
        return await message
            .ReceiveStreamAsync()
            .MatchUnsafeAsync(stream => stream, _ => null)
            .ConfigureAwait(false);
    }
}