using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Essentials.HttpClient.Helpers;

/// <summary>
/// Хелперы для работы с Http запросами
/// </summary>
internal static class HttpRequestsHelpers
{
    /// <summary>
    /// Возвращает хедер типа содержимого application/json
    /// </summary>
    /// <param name="charset">Кодировка</param>
    /// <returns></returns>
    public static MediaTypeHeaderValue GetJsonMediaTypeHeader(string? charset) =>
        new(MediaTypeNames.Application.Json, charset ?? Encoding.Default.WebName);
    
    /// <summary>
    /// Возвращает хедер типа содержимого application/xml
    /// </summary>
    /// <param name="charset">Кодировка</param>
    /// <returns></returns>
    public static MediaTypeHeaderValue GetApplicationXmlMediaTypeHeader(string? charset) =>
        new(MediaTypeNames.Application.Xml, charset ?? Encoding.Default.WebName);
    
    /// <summary>
    /// Возвращает хедер типа содержимого text/xml
    /// </summary>
    /// <param name="charset">Кодировка</param>
    /// <returns></returns>
    public static MediaTypeHeaderValue GetTextXmlMediaTypeHeader(string? charset) =>
        new(MediaTypeNames.Text.Xml, charset ?? Encoding.Default.WebName);
}