using Essentials.HttpClient.ContentTypes.Interfaces;

namespace Essentials.HttpClient.ContentTypes;

/// <inheritdoc cref="IContentType" />
public class JsonContentType : IContentType
{
    /// <inheritdoc cref="IContentType.ContentTypeName" />
    public string ContentTypeName => "Json";
}