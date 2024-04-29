using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IResponse" />
internal record Response(IRequest Request, HttpResponseMessage ResponseMessage) : IResponse
{
    /// <inheritdoc cref="IResponse.Request" />
    public IRequest Request { get; } = Request;
    
    /// <inheritdoc cref="IResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; } = ResponseMessage.CheckNotNull();
}