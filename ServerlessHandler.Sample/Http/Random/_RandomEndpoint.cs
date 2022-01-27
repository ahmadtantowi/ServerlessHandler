using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ServerlessHandler.Sample.Http.Random.Model;

namespace ServerlessHandler.Sample.Http.Random;

public class _RandomEndpoint
{
    private const string _route = "random";
    private const string _tag = "Random Generator";

    [FunctionName(nameof(GetRandomNumber))]
    [OpenApiOperation(nameof(GetRandomNumber), _tag)]
    [OpenApiParameter(nameof(GetRandomNumberRequest.Min), In = ParameterLocation.Query, Type = typeof(int?))]
    [OpenApiParameter(nameof(GetRandomNumberRequest.Max), In = ParameterLocation.Query, Type = typeof(int?))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(int))]
    public async Task<IActionResult> GetRandomNumber(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = _route + "/number")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var handler = request.HttpContext.RequestServices.GetService<GetRandomNumberHandler>();
        var result = await handler.Execute(request, cancellationToken);

        return new OkObjectResult(result);
    }

    [FunctionName(nameof(GetRandomId))]
    [OpenApiOperation(nameof(GetRandomId), _tag)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Guid))]
    public Task<IActionResult> GetRandomId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = _route + "/id")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var handler = request.HttpContext.RequestServices.GetService<GetRandomIdHandler>();

        return handler.Execute(request, cancellationToken);
    }
}
