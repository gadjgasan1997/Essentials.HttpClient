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
    /// <returns>Строка ответа</returns>
    public static TaskAwaiter<string?> GetAwaiter(this string address)
    {
        if (HttpClientsHolder.PeekUnsafe() is not { } client)
            throw new InvalidOperationException($"Не найден http клиент для отправки запроса по адресу: '{address}'");
            
        return client.GetStringAsync(address).GetAwaiter();
    }
}