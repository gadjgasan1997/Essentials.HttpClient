using System.Text;

namespace Essentials.HttpClient.Models.Implementations;

/// <inheritdoc cref="IEssentialsHttpRequest" />
internal record EssentialsHttpRequest(
    string ClientName,
    HttpRequestMessage RequestMessage,
    TimeSpan? Timeout,
    Encoding Encoding) : IEssentialsHttpRequest
{
    /// <inheritdoc cref="IEssentialsHttpRequest.ClientName" />
    public string ClientName { get; } =
        ClientName ?? throw new ArgumentNullException(nameof(ClientName));

    /// <inheritdoc cref="IEssentialsHttpRequest.RequestMessage" />
    public HttpRequestMessage RequestMessage { get; } =
        RequestMessage ?? throw new ArgumentNullException(nameof(RequestMessage));

    /// <inheritdoc cref="IEssentialsHttpRequest.Timeout" />
    public TimeSpan? Timeout { get; } = Timeout;

    /// <inheritdoc cref="IEssentialsHttpRequest.Encoding" />
    public Encoding Encoding { get; } = Encoding;
}