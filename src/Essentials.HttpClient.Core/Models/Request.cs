﻿using LanguageExt;
using LanguageExt.Common;
using System.Text;
using System.Net.Http.Headers;
using System.Diagnostics.Contracts;
using Essentials.Serialization;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Serialization;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Events.EventsStorage;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IRequest" />
internal class Request : IRequest
{
    public Request(
        RequestId id,
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
        Id = id.CheckNotNull("Id запроса не может быть пустым");
        ClientName = clientName.CheckNotNullOrEmpty("Название клиента не может быть пустым");
        TypeId = typeId;
        Uri = uri.CheckNotNull("Uri запроса не может быть пустым");
        MediaType = mediaType;
        Timeout = timeout;
        MetricsOptions = metricsOptions;
        ModifyRequestActions = modifyRequestActions ?? System.Array.Empty<Action<HttpRequestMessage>>();
        Interceptors = interceptors ?? System.Array.Empty<Type>();
        EventsHandlers = eventsHandlers ?? new Dictionary<string, Handler>();
    }
    
    /// <inheritdoc cref="IRequest.Id" />
    public RequestId Id { get; }

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

    /// <inheritdoc cref="IRequest.BuildStreamContent{TData, TSerializer}" />
    public Validation<Error, StreamContent> BuildStreamContent<TData, TSerializer>(
        TData data,
        string? serializerKey = null)
        where TSerializer : IEssentialsSerializer
    {
        var validation = string.IsNullOrWhiteSpace(serializerKey)
            ? SerializersManager.GetSerializer<TSerializer>()
            : SerializersManager.GetSerializer<TSerializer>(serializerKey);
        
        return validation
            .Bind(serializer =>
                SerializeObject(serializer, data)
                    .Map(bytes => new MemoryStream(bytes))
                    .Map(stream => new StreamContent(stream)));
    }
    
    private Validation<Error, byte[]> SerializeObject<T>(
        IEssentialsSerializer serializer,
        T content)
    {
        using var scope = HttpRequestContext.CreateContext(this);

        return TryOption(() => serializer.Serialize(content))
            .Match(
                Success<Error, byte[]>,
                None: () => OnNone(),
                Fail: exception => OnFail(exception));

        Error OnNone()
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(new InvalidOperationException(SerializeNull), SerializeNull);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnSerializeError), RaiseOnSerializeError);
            
            return Error.New(SerializeNull);
        }

        Error OnFail(Exception exception)
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(exception, SerializeError);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnSerializeError), RaiseOnSerializeError);
            
            return Error.New(exception);
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append("{ ")
            .Append("\"Id запроса\": \"")
            .Append(Id.Value)
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