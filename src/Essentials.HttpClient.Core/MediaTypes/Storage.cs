using Essentials.HttpClient.MediaTypes.Application;

namespace Essentials.HttpClient.MediaTypes;

/// <summary>
/// Известные типы содержимого
/// </summary>
public static class Storage
{
    /// <summary>
    /// Application
    /// </summary>
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