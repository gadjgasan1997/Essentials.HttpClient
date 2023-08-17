using System.Collections.Concurrent;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Cache;

/// <summary>
/// Сервис кеширования Uri
/// </summary>
internal static class UriCacheService
{
    private static readonly ConcurrentDictionary<string, Uri> _uris = new();

    /// <summary>
    /// Список Id Uri, которые не должны кешироваться
    /// </summary>
    public static List<string> IgnoredIdList { private get; set; } = new();
    
    /// <summary>
    /// Возвращает Uri из кеша по Id или создает новую
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания Uri</param>
    /// <returns></returns>
    public static Validation<Error, Uri> GetFromCacheOrCreate(string id, Func<Validation<Error, Uri>> creator)
    {
        // TODO Log
        if (IgnoredIdList.Contains(id))
            return creator();
        
        if (_uris.TryGetValue(id, out var cachedUri))
            return cachedUri;

        return creator()
            .Bind<Uri>(uri =>
            {
                if (!_uris.TryAdd(id, uri))
                    return Error.New($"Ошибка кеширования адреса запроса: '{uri}'");
                return uri;
            });
    }
}