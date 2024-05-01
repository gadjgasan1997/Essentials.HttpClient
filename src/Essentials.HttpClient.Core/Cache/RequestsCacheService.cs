using LanguageExt;
using LanguageExt.Common;
using System.Collections.Concurrent;
using static Essentials.HttpClient.Logging.LogManager;
// ReSharper disable InvertIf

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
    public static List<string> IgnoredIdList { private get; set; } = [];
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Validation<Error, IRequest> GetFromCacheOrCreate(string id, Func<Validation<Error, IRequest>> creator)
    {
        if (IgnoredIdList.Contains(id))
        {
            MainLogger.Trace($"Запрос с Id '{id}' не кешируется. Он каждый раз будет создаваться заново");
            return creator();
        }

        if (_requests.TryGetValue(id, out var cachedRequest))
        {
            cachedRequest.Id.RefreshIdIfDefault();
            
            MainLogger.Trace(
                $"Запрос по CacheId '{id}' был успешно получен из кеша. " +
                $"Запросу был присвоен новый RequestId: '{cachedRequest.Id.Value}'");
            
            return Validation<Error, IRequest>.Success(cachedRequest);
        }

        return creator()
            .Bind<IRequest>(request =>
            {
                if (!_requests.TryAdd(id, request))
                    return Error.New($"Ошибка кеширования запроса с Uri '{request.Uri}' и Id '{id}'");
                
                MainLogger.Trace(
                    $"Запрос с Id '{id}' был успешно создан и закеширован. " +
                    $"При следующем запросе он будет браться из кеша");
                
                return Validation<Error, IRequest>.Success(request);
            });
    }
}