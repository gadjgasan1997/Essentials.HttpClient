using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.MediaTypes.Application;
using ApplicationXml = Essentials.HttpClient.MediaTypes.Application.Xml;

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
}