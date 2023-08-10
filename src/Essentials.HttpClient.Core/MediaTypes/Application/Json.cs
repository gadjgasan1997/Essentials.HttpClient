using Essentials.HttpClient.MediaTypes.Interfaces;

namespace Essentials.HttpClient.MediaTypes.Application;

/// <inheritdoc cref="IMediaType" />
public record Json : IMediaType
{
    /// <inheritdoc cref="IMediaType.TypeName" />
    public string TypeName => "application/json";
}