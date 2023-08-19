using System.Net.Http.Headers;
using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IRequest" />
internal record Request(
    string ClientName,
    Uri Uri,
    MediaTypeHeaderValue? MediaTypeHeader = null,
    AuthenticationHeaderValue? AuthenticationHeader = null,
    IEnumerable<(string, IEnumerable<string?>)>? Headers = null,
    IEnumerable<Action<HttpRequestMessage>>? ModifyRequestActions = null,
    TimeSpan? Timeout = null) : IRequest
{
    /// <inheritdoc cref="IRequest.ClientName" />
    public string ClientName { get; } = ClientName.CheckNotNullOrEmpty();

    /// <inheritdoc cref="IRequest.Uri" />
    public Uri Uri { get; } = Uri.CheckNotNull();

    /// <inheritdoc cref="IRequest.MediaTypeHeader" />
    public MediaTypeHeaderValue? MediaTypeHeader { get; set; } = MediaTypeHeader;
    
    /// <inheritdoc cref="IRequest.AuthenticationHeader" />
    public AuthenticationHeaderValue? AuthenticationHeader { get; } = AuthenticationHeader;

    /// <inheritdoc cref="IRequest.Headers" />
    public IEnumerable<(string Name, IEnumerable<string?> Values)>? Headers { get; } = Headers;

    /// <inheritdoc cref="IRequest.ModifyRequestActions" />
    public IEnumerable<Action<HttpRequestMessage>>? ModifyRequestActions { get; } = ModifyRequestActions;

    /// <inheritdoc cref="IRequest.Timeout" />
    public TimeSpan? Timeout { get; } = Timeout;
}