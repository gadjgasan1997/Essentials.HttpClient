using Essentials.HttpClient.ContentTypes.Interfaces;

namespace Essentials.HttpClient.ContentTypes;

/// <inheritdoc cref="IContentType" />
public class ApplicationJson : IContentType
{
    /// <inheritdoc cref="IContentType.ContentTypeName" />
    public string ContentTypeName => "application/json";
}