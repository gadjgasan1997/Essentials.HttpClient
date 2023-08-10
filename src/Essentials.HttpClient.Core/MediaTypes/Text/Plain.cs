using Essentials.HttpClient.MediaTypes.Interfaces;

namespace Essentials.HttpClient.MediaTypes.Text;

/// <inheritdoc cref="IMediaType" />
public record Plain : IMediaType
{
    /// <inheritdoc cref="IMediaType.TypeName" />
    public string TypeName => "text/plain";
}