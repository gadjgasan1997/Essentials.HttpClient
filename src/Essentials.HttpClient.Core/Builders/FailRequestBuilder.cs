using System.Text;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Builders;

/// <summary>
/// Билдер запроса, использующийся в случае ошибок при его инициализации
/// </summary>
internal class FailRequestBuilder : IRequestBuilder
{
    private const string ERROR_MESSAGE = "Во время создания билдера запроса произошло исключение";
    
    private readonly Validation<Error, IRequest> _failResult;

    public FailRequestBuilder(Exception ex)
    {
        _failResult = Fail<Error, IRequest>(Error.New(ERROR_MESSAGE, ex));
    }

    public FailRequestBuilder(Seq<Error> errors)
    {
        _failResult = Fail<Error, IRequest>(errors); 
    }

    public IRequestBuilder WithHeader(string name, params string?[] values) => this;

    public IRequestBuilder WithNotEmptyHeader(string name, params string?[] values) => this;

    public IRequestBuilder WithHeaders(params (string Name, IEnumerable<string?> Value)[] headers) => this;

    public IRequestBuilder WithNotEmptyHeaders(params (string Name, IEnumerable<string?> Value)[] headers) => this;
    
    public IRequestBuilder SetMediaType(string mediaType, Encoding? encoding = null) => this;

    public IRequestBuilder WithBasicAuthentication(string userName, string password) => this;

    public IRequestBuilder WithJwtAuthentication(string token) => this;

    public IRequestBuilder WithAuthentication(string scheme, string parameter) => this;

    public IRequestBuilder ModifyRequest(Action<HttpRequestMessage?> action) => this;

    public IRequestBuilder SetTimeout(TimeSpan timeout) => this;

    public Validation<Error, IRequest> Build(string? clientName = null) => _failResult;

    public Task<Validation<Error, IRequest>> BuildAsync(string? clientName = null) => _failResult.AsTask();

    public Validation<Error, IRequest> Build<TClient>() => _failResult;

    public Task<Validation<Error, IRequest>> BuildAsync<TClient>() => _failResult.AsTask();
}