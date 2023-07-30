namespace Essentials.HttpClient.Models.Implementations;

/// <inheritdoc cref="IHttpRequest" />
internal record HttpRequest(string ClientName, HttpRequestMessage RequestMessage) : IHttpRequest
{
    /// <inheritdoc cref="IHttpRequest.ClientName" />
    public string ClientName { get; } =
        ClientName ?? throw new ArgumentNullException(nameof(ClientName));

    /// <inheritdoc cref="IHttpRequest.RequestMessage" />
    public HttpRequestMessage RequestMessage { get; } =
        RequestMessage ?? throw new ArgumentNullException(nameof(RequestMessage));
}