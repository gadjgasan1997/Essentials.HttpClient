using System.Runtime.CompilerServices;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения на строках
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Осуществляет Http запрос по адресу, возвращая строку в качестве результата
    /// </summary>
    /// <param name="address">Адрес</param>
    /// <returns></returns>
    public static TaskAwaiter<string?> GetAwaiter(this string address) =>
        HttpClientsHolder.PeekUnsafe()?.GetStringAsync(address).GetAwaiter() ?? new TaskAwaiter<string?>();
}