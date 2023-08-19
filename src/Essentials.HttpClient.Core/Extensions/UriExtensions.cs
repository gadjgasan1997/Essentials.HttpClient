namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="Uri" />
/// </summary>
internal static class UriExtensions
{
    /// <summary>
    /// Проверяет адрес на валидность
    /// </summary>
    /// <param name="uri">Адрес</param>
    /// <exception cref="ArgumentException"></exception>
    public static void Validate(this Uri uri)
    {
        if (!Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
            throw new ArgumentException($"Invalid uri format: '{uri}'", nameof(uri));
    }
}