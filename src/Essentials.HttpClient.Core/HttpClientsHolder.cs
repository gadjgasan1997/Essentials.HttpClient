using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient;

/// <summary>
/// Класс для хранения Http клиентов
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class HttpClientsHolder
{
    private static readonly ConcurrentStack<IEssentialsHttpClient> _clients = new();

    /// <summary>
    /// Добавляет Http клиент
    /// </summary>
    /// <param name="essentialsHttpClient"></param>
    internal static void Push(IEssentialsHttpClient essentialsHttpClient) => _clients.Push(essentialsHttpClient);

    /// <summary>
    /// Возвращает Http клиент
    /// </summary>
    /// <returns></returns>
    public static Validation<Error, IEssentialsHttpClient> Peek() =>
        _clients.TryPeek(out var client)
            ? Success<Error, IEssentialsHttpClient>(client)
            : Error.New("Во время получения Http клиента из холдера произошла ошибка. Клиент не найден.");

    /// <summary>
    /// Возвращает Http клиент
    /// </summary>
    /// <returns></returns>
    public static IEssentialsHttpClient? PeekUnsafe() => Peek().MatchUnsafe(client => client, _ => null!);
}