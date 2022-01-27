using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerlessHandler.Abstractions;

namespace ServerlessHandler.Sample.Http.Random;

public class GetRandomIdHandler : IHttpHandler
{
    public Task<IActionResult> Execute(HttpRequest request, CancellationToken cancellationToken = default, params KeyValuePair<string, object>[] keyValues)
    {
        var result = new OkObjectResult(Guid.NewGuid());

        return Task.FromResult(result as IActionResult);
    }
}
