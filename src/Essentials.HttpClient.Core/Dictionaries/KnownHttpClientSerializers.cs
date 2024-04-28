namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Известные сериалайзеры, использующиеся для Http клиентов
/// </summary>
internal static class KnownHttpClientSerializers
{
    /// <summary>
    /// Newtonsoft.Json
    /// </summary>
    public const string NEWTONSOFT_JSON = "HttpClientNewtonsoftJsonSerializer";
    
    /// <summary>
    /// System.Text.Json
    /// </summary>
    public const string NATIVE_JSON = "HttpClientNativeJsonSerializer";
    
    /// <summary>
    /// Xml
    /// </summary>
    public const string XML = "HttpClientXmlSerializer";
}