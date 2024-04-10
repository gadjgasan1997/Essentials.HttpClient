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
    public static MediaTypeHeaderValue GetJsonMediaTypeHeader(string? charset = null) =>
        new(MediaTypeNames.Application.Json) { CharSet = charset ?? Encoding.Default.WebName };

    /// <summary>
    /// Возвращает хедер типа содержимого application/xml
    /// </summary>
    /// <param name="charset">Кодировка</param>
    /// <returns></returns>
    public static MediaTypeHeaderValue GetXmlMediaTypeHeader(string? charset = null) =>
        new(MediaTypeNames.Application.Xml) { CharSet = charset ?? Encoding.Default.WebName };
}