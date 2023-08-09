using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IEssentialsHttpResponse" />
internal record EssentialsHttpResponse(HttpResponseMessage ResponseMessage) : IEssentialsHttpResponse
{
    /// <inheritdoc cref="IEssentialsHttpResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; } = ResponseMessage.CheckNotNull();
}