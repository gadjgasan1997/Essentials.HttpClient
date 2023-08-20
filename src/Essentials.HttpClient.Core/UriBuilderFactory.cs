using Essentials.HttpClient.Builders;
using Essentials.HttpClient.Events.Args;
using Essentials.HttpClient.Extensions;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;
using static Essentials.HttpClient.Events.EventsPublisher;

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
        catch (Exception exception)
        {
            RaiseOnErrorCreateUri(
                new ErrorCreateUriEventArgs(
                    address,
                    exception,
                    string.Format(ErrorCreateUri, exception.Message)));
            
            return new FailUriBuilder(exception);
        }
    }

    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="uri">Базовый адрес запроса</param>
    public static IUriBuilder CreateBuilder(Uri uri) => CreateBuilder(uri.ToString());
}