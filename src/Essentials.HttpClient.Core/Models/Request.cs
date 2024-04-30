using LanguageExt;
using System.Net.Http.Headers;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IRequest" />
internal record Request(
    string Id,
    string ClientName,
    Option<string> TypeId,
    Uri Uri,
    Option<MediaTypeHeaderValue> MediaType,
    Option<TimeSpan> Timeout,
    Option<RequestMetricsOptions> MetricsOptions,
    IEnumerable<Action<HttpRequestMessage>>? ModifyRequestActions = null,
    IEnumerable<Type>? Interceptors = null,
    Dictionary<string, Handler>? EventsHandlers = null) : IRequest
{
    /// <inheritdoc cref="IRequest.Id" />
    public string Id { get; } = Id.CheckNotNullOrEmpty("Id запроса не может быть пустым");

    /// <inheritdoc cref="IRequest.ClientName" />
    public string ClientName { get; } = ClientName.CheckNotNullOrEmpty("Название клиента не может быть пустым");

    /// <inheritdoc cref="IRequest.TypeId" />
    public Option<string> TypeId { get; } = TypeId;

    /// <inheritdoc cref="IRequest.Uri" />
    public Uri Uri { get; } = Uri.CheckNotNull("Uri запроса не может быть пустым");

    /// <inheritdoc cref="IRequest.MediaType" />
    public Option<MediaTypeHeaderValue> MediaType { get; set; } = MediaType;

    /// <inheritdoc cref="IRequest.Timeout" />
    public Option<TimeSpan> Timeout { get; } = Timeout;

    /// <inheritdoc cref="IRequest.MetricsOptions" />
    public Option<RequestMetricsOptions> MetricsOptions { get; } = MetricsOptions;

    /// <inheritdoc cref="IRequest.ModifyRequestActions" />
    public IEnumerable<Action<HttpRequestMessage>> ModifyRequestActions { get; } =
        ModifyRequestActions ?? Array.Empty<Action<HttpRequestMessage>>();

    /// <inheritdoc cref="IRequest.Interceptors" />
    public IEnumerable<Type> Interceptors { get; } = Interceptors ?? Array.Empty<Type>();

    /// <inheritdoc cref="IRequest.EventsHandlers" />
    public Dictionary<string, Handler> EventsHandlers { get; } =
        EventsHandlers ?? new Dictionary<string, Handler>();
}