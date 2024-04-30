using LanguageExt;
using System.Net.Http.Headers;
using System.Text;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IRequest" />
internal record Request : IRequest
{
    public Request(
        string id,
        string clientName,
        Option<string> typeId,
        Uri uri,
        Option<MediaTypeHeaderValue> mediaType,
        Option<TimeSpan> timeout,
        Option<RequestMetricsOptions> metricsOptions,
        IEnumerable<Action<HttpRequestMessage>>? modifyRequestActions = null,
        IEnumerable<Type>? interceptors = null,
        Dictionary<string, Handler>? eventsHandlers = null)
    {
        Id = id.CheckNotNullOrEmpty("Id запроса не может быть пустым");
        ClientName = clientName.CheckNotNullOrEmpty("Название клиента не может быть пустым");
        TypeId = typeId;
        Uri = uri.CheckNotNull("Uri запроса не может быть пустым");
        MediaType = mediaType;
        Timeout = timeout;
        MetricsOptions = metricsOptions;
        ModifyRequestActions = modifyRequestActions ?? Array.Empty<Action<HttpRequestMessage>>();
        Interceptors = interceptors ?? Array.Empty<Type>();
        EventsHandlers = eventsHandlers ?? new Dictionary<string, Handler>();
    }
    
    /// <inheritdoc cref="IRequest.Id" />
    public string Id { get; }

    /// <inheritdoc cref="IRequest.ClientName" />
    public string ClientName { get; }

    /// <inheritdoc cref="IRequest.TypeId" />
    public Option<string> TypeId { get; }

    /// <inheritdoc cref="IRequest.Uri" />
    public Uri Uri { get; }

    /// <inheritdoc cref="IRequest.MediaType" />
    public Option<MediaTypeHeaderValue> MediaType { get; set; }

    /// <inheritdoc cref="IRequest.Timeout" />
    public Option<TimeSpan> Timeout { get; }

    /// <inheritdoc cref="IRequest.MetricsOptions" />
    public Option<RequestMetricsOptions> MetricsOptions { get; }

    /// <inheritdoc cref="IRequest.ModifyRequestActions" />
    public IEnumerable<Action<HttpRequestMessage>> ModifyRequestActions { get; }

    /// <inheritdoc cref="IRequest.Interceptors" />
    public IEnumerable<Type> Interceptors { get; }

    /// <inheritdoc cref="IRequest.EventsHandlers" />
    public Dictionary<string, Handler> EventsHandlers { get; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append("{ ")
            .Append("\"Id запроса\": \"")
            .Append(Id)
            .Append("\", ")
            .Append("\"Название http клиента\": \"")
            .Append(ClientName)
            .Append("\", ")
            .Append("\"Тип запроса\": \"")
            .Append(TypeId)
            .Append("\", ")
            .Append("\"Адрес запроса\": \"")
            .Append(Uri)
            .Append("\" }");
        
        return builder.ToString();
    }
}