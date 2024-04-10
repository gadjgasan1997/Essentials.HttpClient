namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Известные десериалайзеры, использующиеся для Http клиентов
/// </summary>
public static class KnownHttpClientDeserializers
{
    /// <summary>
    /// Newtonsoft.Json
    /// </summary>
    public const string NEWTONSOFT_JSON = "HttpClientNewtonsoftJsonDeserializer";
    
    /// <summary>
    /// System.Text.Json
    /// </summary>
    public const string NATIVE_JSON = "HttpClientNativeJsonDeserializer";
    
    /// <summary>
    /// Xml
    /// </summary>
    public const string XML = "HttpClientXmlDeserializer";
}