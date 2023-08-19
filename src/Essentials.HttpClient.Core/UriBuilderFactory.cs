using Essentials.HttpClient.Builders;
using Essentials.HttpClient.Cache;
using Essentials.HttpClient.Extensions;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

/// <summary>
/// Фабрика для создания адреса запроса
/// </summary>
public static class UriBuilderFactory
{
    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="address">Базовый адрес запроса</param>
    public static IUriBuilder CreateBuilder(string address)
    {
        try
        {
            var uri = new Uri(address);
            uri.Validate();

            return new EssentialsUriBuilder(uri);
        }
        catch (Exception ex)
        {
            // TODO Log
            return new FailUriBuilder(ex);
        }
    }

    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="uri">Базовый адрес запроса</param>
    public static IUriBuilder CreateBuilder(Uri uri) => CreateBuilder(uri.ToString());
    
    /// <summary>
    /// Возвращает Uri из кеша по Id или создает новую
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания Uri</param>
    /// <returns></returns>
    public static Validation<Error, Uri> GetFromCacheOrCreate(
        string id,
        Func<Validation<Error, Uri>> creator)
    {
        return UriCacheService.GetFromCacheOrCreate(id, creator);
    }
    
    /// <summary>
    /// Возвращает Uri из кеша по Id или создает новую
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания Uri</param>
    /// <returns></returns>
    public static Task<Validation<Error, Uri>> GetFromCacheOrCreateAsync(
        string id,
        Func<Task<Validation<Error, Uri>>> creator)
    {
        return UriCacheService.GetFromCacheOrCreate(id, () => creator().Result).AsTask();
    }
}