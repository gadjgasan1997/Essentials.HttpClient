using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Web;
using Essentials.HttpClient.Extensions;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Builders;

/// <inheritdoc cref="IUriBuilder" />
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class EssentialsUriBuilder : IUriBuilder
{
    private const string SEPARATOR = "/";

    /// <inheritdoc cref="IUriBuilder.Segments" />
    public List<string> Segments { get; }
    
    /// <inheritdoc cref="IUriBuilder.UriBuilder" />
    public UriBuilder UriBuilder { get; }
    
    /// <inheritdoc cref="IUriBuilder.Query" />
    public NameValueCollection Query { get; }

    public EssentialsUriBuilder(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        UriBuilder = new UriBuilder(uri);
        Query = HttpUtility.ParseQueryString(UriBuilder.Query);
        Segments = UriBuilder.Path.Split(SEPARATOR).Where(@string => !string.IsNullOrWhiteSpace(@string)).ToList();
    }
    
    /// <inheritdoc cref="IUriBuilder.WithSegment" />
    public IUriBuilder WithSegment(string segment) => ModifyRequest(() => AddSegment(segment));

    /// <inheritdoc cref="IUriBuilder.WithSegments" />
    public IUriBuilder WithSegments(params string[] segments)
    {
        return ModifyRequest(() =>
        {
            foreach (var segment in segments)
                AddSegment(segment);
        });
    }

    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    private void AddSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return;

        Segments.AddRange(segment.Split(SEPARATOR));
    }
    
    /// <inheritdoc cref="IUriBuilder.WithUriParam" />
    public IUriBuilder WithUriParam(string name, string? value) => ModifyRequest(() => AddUriParam(name, value));

    /// <inheritdoc cref="IUriBuilder.WithNotEmptyUriParam" />
    public IUriBuilder WithNotEmptyUriParam(string name, string? value)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AddUriParam(name, value);
        });
    }

    /// <inheritdoc cref="IUriBuilder.WithUriParams" />
    public IUriBuilder WithUriParams(params (string, string?)[] parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
                AddUriParam(name, value);
        });
    }

    /// <inheritdoc cref="IUriBuilder.WithNotEmptyUriParams" />
    public IUriBuilder WithNotEmptyUriParams(params (string, string?)[] parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                AddUriParam(name, value);
            }
        });
    }

    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    private void AddUriParam(string name, string? value)
    {
        Contract.Assert(Query is not null, "Query must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        Query[name] = value;
    }
    
    /// <inheritdoc cref="IUriBuilder.Build" />
    public Validation<Error, Uri> Build()
    {
        try
        {
            UriBuilder.Path = string.Join(SEPARATOR, Segments);
            UriBuilder.Query = Query.ToString();
            
            var uri = new Uri(UriBuilder.ToString());
            uri.Validate();
            
            // TODO Log Trace
            return uri;
        }
        catch (Exception ex)
        {
            var message = $"Во время построения адреса запроса произошла ошибка. Некорректный адрес: '{UriBuilder}'.";
            
            // TODO Log
            return Fail<Error, Uri>(message);
        }
    }

    /// <inheritdoc cref="IUriBuilder.BuildAsync" />
    public Task<Validation<Error, Uri>> BuildAsync() => Build().AsTask();

    /// <summary>
    /// Меняет адрес запроса
    /// </summary>
    /// <param name="modifyAddressAction">Действие изменения адреса</param>
    /// <returns></returns>
    private EssentialsUriBuilder ModifyRequest(Action modifyAddressAction)
    {
        // TODO Log if fail
        _ = Try(() =>
            {
                modifyAddressAction();
                return this;
            })
            .Try()
            .IfFail(_ => { });
        
        return this;
    }
}