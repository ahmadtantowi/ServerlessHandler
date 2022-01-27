using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerlessHandler.Abstractions
{
    public interface IHttpHandler<TResult> : IHandler
    {
        Task<TResult> Execute(HttpRequest request, CancellationToken cancellationToken = default, params KeyValuePair<string, object>[] keyValues);
    }

    public interface IHttpHandler : IHttpHandler<IActionResult> {}
}