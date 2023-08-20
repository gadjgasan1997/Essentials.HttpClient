using Microsoft.AspNetCore.Mvc;

namespace Essentials.HttpClient.Sample.Server.Helpers;

public static class ResponseHelpers
{
    public static ContentResult GetErrorContent(Exception exception) =>
        new()
        {
            StatusCode = 500,
            Content = exception.Message
        };
}