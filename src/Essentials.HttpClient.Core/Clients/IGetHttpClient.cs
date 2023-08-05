using Essentials.HttpClient.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Clients;

public interface IGetHttpClient
{
    Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default);
}