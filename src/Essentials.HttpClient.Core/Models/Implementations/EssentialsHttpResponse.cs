namespace Essentials.HttpClient.Models.Implementations;

/// <inheritdoc cref="IEssentialsHttpResponse" />
internal record EssentialsHttpResponse(HttpResponseMessage ResponseMessage) : IEssentialsHttpResponse
{
    /// <inheritdoc cref="IEssentialsHttpResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; } =
        ResponseMessage ?? throw new ArgumentNullException(nameof(ResponseMessage));
}