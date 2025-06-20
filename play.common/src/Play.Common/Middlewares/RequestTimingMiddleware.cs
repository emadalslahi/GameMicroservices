using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Play.Common.Middlewares;

public class RequestTimingMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        var stopWatch = new Stopwatch();
        try
        {
            stopWatch.Start();
            await _next(context);
        }
        finally
        {
            stopWatch.Stop();
            _logger.LogInformation("{RequestMethod} {requestPath} request took {elpsdMlscnd}"
            , context.Request.Method
            , context.Request.Path, stopWatch.ElapsedMilliseconds);
        }

    }

}