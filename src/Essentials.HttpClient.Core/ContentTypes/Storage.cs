using Essentials.HttpClient.ContentTypes.Application;

namespace Essentials.HttpClient.ContentTypes;

/// <summary>
/// Известные типы содержимого
/// </summary>
public static class Storage
{
    public static class Application
    {
        /// <summary>
        /// application/json
        /// </summary>
        public static Json Json { get; } = new();
    
        /// <summary>
        /// application/xml
        /// </summary>
        public static Xml Xml { get; } = new();
    }
}