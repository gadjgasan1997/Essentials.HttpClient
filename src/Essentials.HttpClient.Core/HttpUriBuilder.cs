using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Web;
using LanguageExt;
using LanguageExt.Common;
using Essentials.HttpClient.Extensions;
using Essentials.Utils.Extensions;
using static Essentials.HttpClient.Dictionaries.CommonConsts;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер для создания Uri
/// </summary>
public class HttpUriBuilder
{
    /// <summary>
    /// Список сегментов запроса
    /// </summary>
    public List<string> Segments { get; }
    
    /// <summary>
    /// Билдер
    /// </summary>
    public UriBuilder UriBuilder { get; }
    
    /// <summary>
    /// Параметры запроса
    /// </summary>
    public NameValueCollection Query { get; }

    private HttpUriBuilder(Uri uri)
    {
        UriBuilder = new UriBuilder(uri);
        Query = HttpUtility.ParseQueryString(UriBuilder.Query);
        Segments = UriBuilder.Path.Split(SEPARATOR).Where(@string => !string.IsNullOrWhiteSpace(@string)).ToList();
    }
    
    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="uri">Базовый адрес запроса</param>
    public static Validation<Error, HttpUriBuilder> CreateBuilder(Uri uri) => CreateBuilder(uri.ToString);

    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="address">Базовый адрес запроса</param>
    public static Validation<Error, HttpUriBuilder> CreateBuilder(string address) => CreateBuilder(() => address);

    /// <summary>
    /// Создает билдер из делегата получения Uri
    /// </summary>
    /// <param name="func">Делегат</param>
    /// <returns></returns>
    private static Validation<Error, HttpUriBuilder> CreateBuilder(Func<string> func)
    {
        try
        {
            var address = func();
            
            address.CheckNotNullOrEmpty();
            
            var uri = new Uri(address);
            uri.Validate();

            return new HttpUriBuilder(uri);
        }
        catch (Exception ex)
        {
            return Error.New(ex);
        }
    }

    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    internal void AddSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return;

        Segments.AddRange(segment.Split(SEPARATOR));
    }

    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    internal void AddUriParam(string name, string? value)
    {
        Contract.Assert(Query is not null, "Query must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        Query[name] = value;
    }
}