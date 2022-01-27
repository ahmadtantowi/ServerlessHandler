using Microsoft.AspNetCore.Http;
using ServerlessHandler.Abstractions;
using ServerlessHandler.Extensions;
using ServerlessHandler.Sample.Http.Random.Model;

namespace ServerlessHandler.Sample.Http.Random;

public class GetRandomNumberHandler : IHttpHandler<int>
{
    private static readonly System.Random _random = new();

    public Task<int> Execute(HttpRequest request, CancellationToken cancellationToken = default, params KeyValuePair<string, object>[] keyValues)
    {
        var param = request.GetParams<GetRandomNumberRequest>();
        int result;

        if (param.Min.HasValue && param.Max.HasValue)
            result = _random.Next(param.Min.Value, param.Max.Value);
        else if (param.Max.HasValue)
            result = _random.Next(param.Max.Value);
        else
            result = _random.Next();

        return Task.FromResult(result);
    }
}
