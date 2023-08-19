using System.Collections.Specialized;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Builders;

/// <summary>
/// Билдер адреса запроса, использующийся в случае ошибок при его инициализации
/// </summary>
internal class FailUriBuilder : IUriBuilder
{
    private const string ERROR_MESSAGE = "Во время создания билдера адреса запроса произошло исключение";
    
    private readonly Validation<Error, Uri> _failResult;
    
    public FailUriBuilder(Exception ex)
    {
        _failResult = Fail<Error, Uri>(Error.New(ERROR_MESSAGE, ex));
    }
    
    public List<string> Segments { get; } = new();
    
    public UriBuilder? UriBuilder => null;
    
    public NameValueCollection? Query => null;
    
    public IUriBuilder WithSegment(string segment) => this;

    public IUriBuilder WithSegments(params string[] segments) => this;

    public IUriBuilder WithUriParam(string name, string? value) => this;

    public IUriBuilder WithNotEmptyUriParam(string name, string? value) => this;

    public IUriBuilder WithUriParams(params (string, string?)[] parameters) => this;

    public IUriBuilder WithNotEmptyUriParams(params (string, string?)[] parameters) => this;

    public Validation<Error, Uri> Build() => _failResult;

    public Task<Validation<Error, Uri>> BuildAsync() => _failResult.AsTask();
}