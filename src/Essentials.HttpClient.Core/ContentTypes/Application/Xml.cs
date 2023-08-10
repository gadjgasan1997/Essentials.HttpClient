using Essentials.HttpClient.ContentTypes.Interfaces;

namespace Essentials.HttpClient.ContentTypes.Application;

/// <inheritdoc cref="IContentType" />
public record Xml : IContentType
{
    /// <inheritdoc cref="IContentType.ContentTypeName" />
    public string ContentTypeName => "application/xml";
}