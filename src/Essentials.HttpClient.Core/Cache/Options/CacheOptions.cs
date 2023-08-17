using System.Diagnostics.CodeAnalysis;

namespace Essentials.HttpClient.Cache.Options;

/// <summary>
/// Опции кеша
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class CacheOptions
{
    /// <summary>
    /// Опции кеша Uri
    /// </summary>
    public UriCacheOptions? Uris { get; set; }
    
    /// <summary>
    /// Опции кеша Uri
    /// </summary>
    public class UriCacheOptions
    {
        /// <summary>
        /// Список Id Uri, которые не должны кешироваться
        /// </summary>
        public List<string> IgnoredIdList { get; set; } = new();
    }
}