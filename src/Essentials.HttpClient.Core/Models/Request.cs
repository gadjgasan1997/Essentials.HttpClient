using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IRequest" />
internal record Request(
    string ClientName,
    HttpRequestMessage RequestMessage,
    TimeSpan? Timeout) : IRequest
{
    /// <inheritdoc cref="IRequest.ClientName" />
    public string ClientName { get; } = ClientName.CheckNotNullOrEmpty();

    /// <inheritdoc cref="IRequest.RequestMessage" />
    public HttpRequestMessage RequestMessage { get; } = RequestMessage.CheckNotNull();
    
    /// <inheritdoc cref="IRequest.Timeout" />
    public TimeSpan? Timeout { get; } = Timeout;
}