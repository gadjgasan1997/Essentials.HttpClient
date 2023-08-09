using System.Text;
using Essentials.HttpClient.ContentTypes.Interfaces;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

public interface IEssentialsHttpClient
{
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default);
    
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = default);

    Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType;

    Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        IEssentialsHttpRequest request,
        string content,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType;

    Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TData>(
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType;

    Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TData>(
        IEssentialsHttpRequest request,
        TData data,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType;
}