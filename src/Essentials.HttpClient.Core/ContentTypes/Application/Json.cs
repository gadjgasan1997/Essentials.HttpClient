using Essentials.HttpClient.ContentTypes.Interfaces;

namespace Essentials.HttpClient.ContentTypes.Application;

/// <inheritdoc cref="IContentType" />
public record Json : IContentType
{
    /// <inheritdoc cref="IContentType.ContentTypeName" />
    public string ContentTypeName => "application/json";
}