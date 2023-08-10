using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.MediaTypes.Application;
using Essentials.HttpClient.MediaTypes.Text;
using ApplicationXml = Essentials.HttpClient.MediaTypes.Application.Xml;
using TextXml = Essentials.HttpClient.MediaTypes.Text.Xml;

namespace Essentials.HttpClient.MediaTypes;

/// <summary>
/// Известные типы содержимого
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class Storage
{
    /// <summary>
    /// Внутренний формат прикладной программы
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
        public static ApplicationXml Xml { get; } = new();
    }
    
    /// <summary>
    /// Текст
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// text/plain
        /// </summary>
        public static Plain Plain { get; } = new();
        
        /// <summary>
        /// text/xml
        /// </summary>
        public static TextXml Xml { get; } = new();
    }
}