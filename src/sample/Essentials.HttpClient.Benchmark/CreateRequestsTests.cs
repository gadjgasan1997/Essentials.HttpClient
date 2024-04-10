using BenchmarkDotNet.Attributes;
using Essentials.HttpClient.Extensions;

namespace Essentials.HttpClient.Benchmark;

[MemoryDiagnoser]
public class CreateRequestsTests
{
    private const int ITERATIONS = 50000;
    
    /// <summary>
    /// Создание запроса без кеша
    /// </summary>
    [Benchmark]
    public void CreateRequestWithoutCache()
    {
        Parallel.For(
            fromInclusive: 1,
            toExclusive: ITERATIONS,
            body: (_, _) => HttpRequestBuilder
                .CreateBuilder(new Uri("http://localhost:5001"))
                .BuildAsync());
    }
    
    /// <summary>
    /// Создание запроса с кешем
    /// </summary>
    [Benchmark]
    public void CreateRequestWithCache()
    {
        Parallel.For(
            fromInclusive: 1,
            toExclusive: ITERATIONS,
            body: (_, _) => HttpRequestBuilder
                .GetFromCacheOrCreate(
                    "id",
                    () => HttpRequestBuilder
                        .CreateBuilder(new Uri("http://localhost:5001"))
                        .BuildAsync()
                        .Result));
    }
}