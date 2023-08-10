namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для валидации объектов
/// </summary>
internal static class CheckExtensions
{
    public static T CheckNotNull<T>(this T item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        return item;
    }
    
    public static string CheckNotNullOrEmpty(this string str, string? errorMessage = "")
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentNullException(nameof(str), errorMessage);
        
        return str;
    }
}