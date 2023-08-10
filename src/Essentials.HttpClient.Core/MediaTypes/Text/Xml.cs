using System.Net.Mime;
using Essentials.HttpClient.MediaTypes.Interfaces;

namespace Essentials.HttpClient.MediaTypes.Text;

/// <inheritdoc cref="IMediaType" />
public record Xml : IMediaType
{
    /// <inheritdoc cref="IMediaType.TypeName" />
    public string TypeName => MediaTypeNames.Text.Xml;
}