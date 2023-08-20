namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для коллекций
/// </summary>
internal static class EnumerableExtensions
{
    /// <summary>
    /// Выполняет действие для всех элементов коллекции
    /// </summary>
    /// <param name="enumerable">Коллекция</param>
    /// <param name="action">Действие</param>
    /// <typeparam name="T"></typeparam>
    public static void Map<T>(this IEnumerable<T> enumerable, Action<T> action) =>
        enumerable.ToList().ForEach(action);

    /// <summary>
    /// Выполняет действие для всех элементов коллекции, удовлетворяющих условию
    /// </summary>
    /// <param name="enumerable">Коллекция</param>
    /// <param name="predicate">Условие</param>
    /// <param name="action">Действие</param>
    /// <typeparam name="T"></typeparam>
    public static void Map<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, Action<T> action) =>
        enumerable.Where(predicate).ToList().ForEach(action);
}