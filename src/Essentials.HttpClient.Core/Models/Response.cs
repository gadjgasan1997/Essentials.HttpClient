using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IResponse" />
internal record Response(HttpResponseMessage ResponseMessage) : IResponse
{
    /// <inheritdoc cref="IResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; } = ResponseMessage.CheckNotNull();
}