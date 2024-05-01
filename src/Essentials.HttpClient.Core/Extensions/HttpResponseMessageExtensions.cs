using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;
using static Essentials.HttpClient.Events.EventsStorage;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="HttpResponseMessage" />
/// </summary>
internal static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Возвращает строку с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <param name="response">Ответ</param>
    /// <returns></returns>
    public static async Task<Validation<Error, string>> ReceiveStringAsync(
        this HttpResponseMessage message,
        IResponse response)
    {
        using var scope = HttpRequestContext.RestoreContext(response);
        
        return await TryOptionAsync(() => message.Content.ReadAsStringAsync())
            .Match(
                Some: Validation<Error, string>.Success,
                None: () => OnNone(),
                Fail: exception => OnFail(exception))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает поток с содержимым из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <param name="response">Ответ</param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ReceiveStreamAsync(
        this HttpResponseMessage message,
        IResponse response)
    {
        using var scope = HttpRequestContext.RestoreContext(response);

        return await TryOptionAsync(() => message.Content.ReadAsStreamAsync())
            .Match(
                Some: Validation<Error, Stream>.Success,
                None: () => OnNone(),
                Fail: exception => OnFail(exception))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает массив байтов из Http ответа
    /// </summary>
    /// <param name="message">Http ответ</param>
    /// <param name="response">Ответ</param>
    /// <returns></returns>
    public static async Task<Validation<Error, byte[]>> ReceiveBytesAsync(
        this HttpResponseMessage message,
        IResponse response)
    {
        using var scope = HttpRequestContext.RestoreContext(response);

        return await TryOptionAsync(() => message.Content.ReadAsByteArrayAsync())
            .Match(
                Some: Validation<Error, byte[]>.Success,
                None: () => OnNone(),
                Fail: exception => OnFail(exception))
            .ConfigureAwait(false);
    }
    
    private static Error OnNone()
    {
        Contract.Assert(HttpRequestContext.Current is not null);
        
        HttpRequestContext.Current.SetError(new InvalidOperationException(ContentIsNull), ContentIsNull);
        
        HttpRequestContext.Current.Request.RaiseEvent(nameof(OnErrorReadContent), RaiseOnErrorReadContent);
        return Error.New(ContentIsNull);   
    }
        
    private static Error OnFail(Exception exception)
    {
        Contract.Assert(HttpRequestContext.Current is not null);

        HttpRequestContext.Current.SetError(exception, string.Format(ErrorGetContent, exception.Message));
        
        HttpRequestContext.Current.Request.RaiseEvent(nameof(OnErrorReadContent), RaiseOnErrorReadContent);
        return Error.New(exception);
    }
}