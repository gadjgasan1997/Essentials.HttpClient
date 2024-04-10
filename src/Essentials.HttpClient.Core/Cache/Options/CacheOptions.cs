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
    /// Опции кеша запросов
    /// </summary>
    public RequestsCacheOptions? Requests { get; set; }
    
    /// <summary>
    /// Опции кеша запросов
    /// </summary>
    public class RequestsCacheOptions
    {
        /// <summary>
        /// Список Id запросов, которые не должны кешироваться
        /// </summary>
        public List<string> IgnoredIdList { get; set; } = new();
    }
}