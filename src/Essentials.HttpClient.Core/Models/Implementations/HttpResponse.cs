namespace Essentials.HttpClient.Models.Implementations;

/// <inheritdoc cref="IHttpResponse" />
internal record HttpResponse(HttpResponseMessage ResponseMessage) : IHttpResponse
{
    /// <inheritdoc cref="IHttpResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; } =
        ResponseMessage ?? throw new ArgumentNullException(nameof(ResponseMessage));
}