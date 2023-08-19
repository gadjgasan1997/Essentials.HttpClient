using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Builders;
using Essentials.HttpClient.Cache;
using Essentials.HttpClient.Extensions;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

/// <summary>
/// Фабрика для создания билдера запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class RequestBuilderFactory
{
    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="uri">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static IRequestBuilder CreateBuilder(Uri uri)
    {
        try
        {
            uri.Validate();
            return new EssentialsRequestBuilder(uri);
        }
        catch (Exception ex)
        {
            // TODO Log
            return new FailRequestBuilder(ex);
        }
    }

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="validation">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static IRequestBuilder CreateBuilder(Validation<Error, Uri> validation) =>
        validation.Match(CreateBuilder, seq => new FailRequestBuilder(seq));
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Validation<Error, IRequest> GetFromCacheOrCreate(
        string id,
        Func<Validation<Error, IRequest>> creator)
    {
        return RequestsCacheService.GetFromCacheOrCreate(id, creator);
    }
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Task<Validation<Error, IRequest>> GetFromCacheOrCreateAsync(
        string id,
        Func<Task<Validation<Error, IRequest>>> creator)
    {
        return RequestsCacheService.GetFromCacheOrCreate(id, () => creator().Result).AsTask();
    }
}