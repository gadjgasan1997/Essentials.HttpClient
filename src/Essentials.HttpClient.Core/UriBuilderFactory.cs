using Essentials.HttpClient.Builders;
using Essentials.HttpClient.Extensions;

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
}