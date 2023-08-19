using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Builders;
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
            return new EssentialsRequestBuilder(new HttpRequestMessage {RequestUri = uri});
        }
        catch (Exception ex)
        {
            // TODO Log
            return new EssentialsRequestBuilder();
        }
    }

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="validation">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static IRequestBuilder CreateBuilder(Validation<Error, Uri> validation) =>
        validation.Match(CreateBuilder, _ => new EssentialsRequestBuilder());
}