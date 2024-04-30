using System.Runtime.CompilerServices;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="Uri" />
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Проверяет адрес на валидность
    /// </summary>
    /// <param name="uri">Адрес</param>
    /// <exception cref="ArgumentException"></exception>
    public static void Validate(this Uri uri)
    {
        uri.CheckNotNull();
        
        if (!Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
            throw new ArgumentException($"Invalid uri format: '{uri}'", nameof(uri));
    }
    
    /// <summary>
    /// Осуществляет Http запрос по адресу, возвращая строку в качестве результата
    /// </summary>
    /// <param name="uri">Адрес</param>
    /// <returns>Строка ответа</returns>
    public static TaskAwaiter<string?> GetAwaiter(this Uri uri)
    {
        if (HttpClientsHolder.PeekUnsafe() is not { } client)
            throw new InvalidOperationException($"Не найден http клиент для отправки запроса по адресу: '{uri}'");
            
        return client.GetStringAsync(uri).GetAwaiter();
    }
}