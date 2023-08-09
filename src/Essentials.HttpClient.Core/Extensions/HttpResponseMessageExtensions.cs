using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

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
}