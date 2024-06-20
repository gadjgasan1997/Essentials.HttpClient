using LanguageExt;
using LanguageExt.Common;
using System.Text;
using System.Diagnostics.Contracts;
using Essentials.Serialization;
using Essentials.Functional.Extensions;
using Essentials.Utils.IO.Extensions;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Serialization;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Events.EventsStorage;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient.Models;

/// <inheritdoc cref="IResponse" />
internal class Response : IResponse
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

    /// <inheritdoc cref="IResponse.ReceiveStringAsync" />
    public async Task<Validation<Error, string>> ReceiveStringAsync()
    {
        using var scope = HttpRequestContext.RestoreContext(this);
        
        return await TryOptionAsync(() => ResponseMessage.Content.ReadAsStringAsync())
            .Match(
                Some: Validation<Error, string>.Success,
                None: () => OnNoneContent(),
                Fail: exception => OnFailReadContent(exception))
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IResponse.ReceiveStreamAsync" />
    public async Task<Validation<Error, Stream>> ReceiveStreamAsync()
    {
        using var scope = HttpRequestContext.RestoreContext(this);

        return await TryOptionAsync(() => ResponseMessage.Content.ReadAsStreamAsync())
            .Match(
                Some: Validation<Error, Stream>.Success,
                None: () => OnNoneContent(),
                Fail: exception => OnFailReadContent(exception))
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IResponse.ReceiveBytesAsync" />
    public async Task<Validation<Error, byte[]>> ReceiveBytesAsync()
    {
        using var scope = HttpRequestContext.RestoreContext(this);

        return await TryOptionAsync(() => ResponseMessage.Content.ReadAsByteArrayAsync())
            .Match(
                Some: Validation<Error, byte[]>.Success,
                None: () => OnNoneContent(),
                Fail: exception => OnFailReadContent(exception))
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IResponse.ReceiveContentAsync{TData, TDeserialize}" />
    public async Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer
    {
        var validation = string.IsNullOrWhiteSpace(deserializerKey)
            ? SerializersManager
                .GetDeserializer<TDeserializer>()
            : SerializersManager
                .GetDeserializer<TDeserializer>(deserializerKey);

        return await validation.BindAsync(DeserializeResponseAsync<TData, TDeserializer>);
    }

    private static Error OnNoneContent()
    {
        Contract.Assert(HttpRequestContext.Current is not null);
        
        HttpRequestContext.Current.SetError(new InvalidOperationException(ContentIsNull), ContentIsNull);
        HttpRequestContext.Current.Request.RaiseEvent(nameof(OnErrorReadContent), RaiseOnErrorReadContent);

        return Error.New(ContentIsNull);
    }
        
    private static Error OnFailReadContent(Exception exception)
    {
        Contract.Assert(HttpRequestContext.Current is not null);

        HttpRequestContext.Current.SetError(exception, string.Format(ErrorGetContent, exception.Message));
        HttpRequestContext.Current.Request.RaiseEvent(nameof(OnErrorReadContent), RaiseOnErrorReadContent);
        
        return Error.New(exception);
    }
    
    private async Task<Validation<Error, TData>> DeserializeResponseAsync<TData, TDeserializer>(
        TDeserializer deserializer)
        where TDeserializer : IEssentialsDeserializer
    {
        return await ReceiveStreamAsync()
            .BindAsync(stream => DeserializeStream<TData>(deserializer, stream))
            .ConfigureAwait(false);
    }
    
    private Validation<Error, TData> DeserializeStream<TData>(
        IEssentialsDeserializer deserializer,
        Stream stream)
    {
        using var scope = HttpRequestContext.RestoreContext(this);

        return Try(stream.AsMemory())
            .Map(Handle)
            .Match(
                Succ: validation => validation,
                Fail: exception => Fail<Error, TData>(OnFail(exception)));

        Validation<Error, TData> Handle(Memory<byte> memory)
        {
            return TryOption(() => deserializer.Deserialize<TData>(memory))
                .Match(
                    Some: data => Success<Error, TData>(data!),
                    None: () => Fail<Error, TData>(OnNone()),
                    Fail: exception => Fail<Error, TData>(OnFail(exception)));
        }
        
        Error OnNone()
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(new InvalidOperationException(DeserializeNull), DeserializeNull);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnDeserializeError), RaiseOnDeserializeError);
            
            return Error.New(DeserializeNull);
        }
        
        Error OnFail(Exception exception)
        {
            Contract.Assert(HttpRequestContext.Current is not null);
            
            HttpRequestContext.Current.SetError(exception, DeserializeError);
            HttpRequestContext.Current.Request.RaiseEvent(nameof(OnDeserializeError), RaiseOnDeserializeError);
            
            return Error.New(exception);
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append("{ ")
            .Append("\"Полученный ответ\": ")
            .Append("{ ")
            .Append("\"Id запроса\": \"")
            .Append(Request.Id.Value)
            .Append("\", ")
            .Append("\"Код ответа\": \"")
            .Append(ResponseMessage.StatusCode)
            .Append("\" }")
            .Append(" }");
        
        return builder.ToString();
    }
}