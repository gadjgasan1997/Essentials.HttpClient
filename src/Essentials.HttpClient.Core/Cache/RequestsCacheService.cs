using System.Collections.Concurrent;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Cache;

/// <summary>
/// Сервис кеширования запросов
/// </summary>
internal static class RequestsCacheService
{
    private static readonly ConcurrentDictionary<string, IRequest> _requests = new();

    /// <summary>
    /// Список Id запросов, которые не должны кешироваться
    /// </summary>
    public static List<string> IgnoredIdList { private get; set; } = new();
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Validation<Error, IRequest> GetFromCacheOrCreate(string id, Func<Validation<Error, IRequest>> creator)
    {
        // TODO Log
        if (IgnoredIdList.Contains(id))
            return creator();
        
        if (_requests.TryGetValue(id, out var cachedRequest))
            return Validation<Error, IRequest>.Success(cachedRequest);

        return creator()
            .Bind<IRequest>(request =>
            {
                if (!_requests.TryAdd(id, request))
                    return Error.New($"Ошибка кеширования запроса с Uri: '{request.Uri}'");
                
                return Validation<Error, IRequest>.Success(request);
            });
    }
}