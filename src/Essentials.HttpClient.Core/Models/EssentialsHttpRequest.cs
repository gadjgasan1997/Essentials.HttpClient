using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IEssentialsHttpRequest" />
internal record EssentialsHttpRequest(
    string ClientName,
    HttpRequestMessage RequestMessage,
    TimeSpan? Timeout) : IEssentialsHttpRequest
{
    /// <inheritdoc cref="IEssentialsHttpRequest.ClientName" />
    public string ClientName { get; } = ClientName.CheckNotNullOrEmpty();

    /// <inheritdoc cref="IEssentialsHttpRequest.RequestMessage" />
    public HttpRequestMessage RequestMessage { get; } = RequestMessage.CheckNotNull();
    
    /// <inheritdoc cref="IEssentialsHttpRequest.Timeout" />
    public TimeSpan? Timeout { get; } = Timeout;
}