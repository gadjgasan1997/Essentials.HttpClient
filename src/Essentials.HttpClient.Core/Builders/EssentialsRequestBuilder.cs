using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Models;
using LanguageExt;
using LanguageExt.Common;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;
using static LanguageExt.Prelude;
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved

namespace Essentials.HttpClient.Builders;

/// <summary>
/// Билдер создания запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class EssentialsRequestBuilder : IRequestBuilder
{
    /// <inheritdoc cref="IRequestBuilder.Timeout"/>
    public TimeSpan? Timeout { get; private set; }
    
    /// <inheritdoc cref="IRequestBuilder.RequestMessage"/>
    public HttpRequestMessage? RequestMessage { get; }

    public EssentialsRequestBuilder(HttpRequestMessage? requestMessage = null)
    {
        RequestMessage = requestMessage;
    }
    
    /// <inheritdoc cref="IRequestBuilder.WithHeader"/>
    public IRequestBuilder WithHeader(string name, params string?[] values) =>
        ModifyRequest(() => AddHeader(name, values));

    /// <inheritdoc cref="IRequestBuilder.WithNotEmptyHeader"/>
    public IRequestBuilder WithNotEmptyHeader(string name, params string?[] values) =>
        ModifyRequest(() => AddHeader(name, values.Where(value => !string.IsNullOrWhiteSpace(value))));
    
    /// <inheritdoc cref="IRequestBuilder.WithHeaders"/>
    public IRequestBuilder WithHeaders(params (string, IEnumerable<string?>)[] headers) =>
        ModifyRequest(() => headers.Map(tuple => AddHeader(tuple.Item1, tuple.Item2)));
    
    /// <inheritdoc cref="IRequestBuilder.WithNotEmptyHeaders"/>
    public IRequestBuilder WithNotEmptyHeaders(params (string, IEnumerable<string?>)[] headers)
    {
        return ModifyRequest(() => headers.Map(tuple =>
            AddHeader(tuple.Item1, tuple.Item2.Where(value => !string.IsNullOrWhiteSpace(value)))));
    }
    
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    private void AddHeader(string name, IEnumerable<string?> values)
    {
        Contract.Assert(RequestMessage is not null, "RequestMessage must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        RequestMessage.Headers.Add(name, values);
    }
    
    /// <inheritdoc cref="IRequestBuilder.SetTimeout"/>
    public IRequestBuilder SetTimeout(TimeSpan timeout) => ModifyRequest(() => Timeout = timeout);
    
    /// <inheritdoc cref="IRequestBuilder.WithBasicAuthentication"/>
    public IRequestBuilder WithBasicAuthentication(string userName, string password)
    {
        var authenticationString = Try(() => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")))
            .Match(
                Succ: @string => @string,
                Fail: exception =>
                {
                    // TODO Log
                    
                    return string.Empty;
                });

        return WithAuthentication(BASIC, authenticationString);
    }
    
    /// <inheritdoc cref="IRequestBuilder.WithJwtAuthentication"/>
    public IRequestBuilder WithJwtAuthentication(string token) => WithAuthentication(JWT, token);
    
    /// <inheritdoc cref="IRequestBuilder.WithAuthentication"/>
    public IRequestBuilder WithAuthentication(string scheme, string parameter)
    {
        return ModifyRequest(() =>
        {
            Contract.Assert(RequestMessage is not null, "RequestMessage must not null here!");

            if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(parameter))
                return;

            RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        });
    }

    /// <inheritdoc cref="IRequestBuilder.ModifyRequest"/>
    public IRequestBuilder ModifyRequest(Action<HttpRequestMessage?> func) =>
        ModifyRequest(() => func(RequestMessage));

    /// <inheritdoc cref="IRequestBuilder.Build"/>
    public Validation<Error, IRequest> Build(string? clientName = null)
    {
        // TODO Rename default client
        return Try(() => BuildPrivate(clientName ?? nameof(IRequestBuilder)))
            .Match(
                Succ: validation => validation,
                Fail: exception =>
                {
                    // TODO Log
                    return Error.New(exception);
                });
    }

    /// <inheritdoc cref="IRequestBuilder.BuildAsync"/>
    public Task<Validation<Error, IRequest>> BuildAsync(string? clientName = null) =>
        Build(clientName).AsTask();

    /// <inheritdoc cref="IRequestBuilder.Build"/>
    public Validation<Error, IRequest> Build<TClient>() => Build(typeof(TClient).Name);

    /// <inheritdoc cref="IRequestBuilder.BuildAsync"/>
    public Task<Validation<Error, IRequest>> BuildAsync<TClient>() => Build<TClient>().AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    private Validation<Error, IRequest> BuildPrivate(string clientName)
    {
        if (RequestMessage is null)
        {
            const string errorMessage = "Не инициализированы обязательные параметры запроса. Запрос не будет создан";

            // TODO Log
            return Fail<Error, IRequest>(errorMessage);
        }

        // TODO Log Trace
        return new Request(clientName, RequestMessage, Timeout);
    }

    /// <summary>
    /// Меняет запрос
    /// </summary>
    /// <param name="modifyRequestAction">Действие по изменению запроса</param>
    /// <returns>Билдер</returns>
    private EssentialsRequestBuilder ModifyRequest(Action modifyRequestAction)
    {
        if (RequestMessage is null)
            return this;

        // TODO Log if fail
        _ = Try(() =>
            {
                modifyRequestAction();
                return this;
            })
            .Try()
            .IfFail(_ => { });
        
        return this;
    }
}