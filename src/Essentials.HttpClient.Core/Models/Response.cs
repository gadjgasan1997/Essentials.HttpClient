using System.Text;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IResponse" />
internal record Response : IResponse
{
    public Response(IRequest request, HttpResponseMessage responseMessage)
    {
        Request = request.CheckNotNull();
        ResponseMessage = responseMessage.CheckNotNull();
    }
    
    /// <inheritdoc cref="IResponse.Request" />
    public IRequest Request { get; }
    
    /// <inheritdoc cref="IResponse.ResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append("{ ")
            .Append("\"Полученный ответ\": ")
            .Append("{ ")
            .Append("\"Id запроса\": \"")
            .Append(Request.Id)
            .Append("\", ")
            .Append("\"Код ответа\": \"")
            .Append(ResponseMessage.StatusCode)
            .Append("\" }")
            .Append(" }");
        
        return builder.ToString();
    }
}