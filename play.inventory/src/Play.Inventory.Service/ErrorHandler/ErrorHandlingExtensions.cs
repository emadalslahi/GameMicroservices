using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Play.Inventory.Service.ErrorHandler;

public static class ErrorHandlingExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Error Handling");

            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionDetails?.Error;

            logger.LogError(exception, "Could not process a request on machin {machinID}. Trace ID: {traceId}",
                    Environment.MachineName,
                    Activity.Current?.TraceId);

            var problem = new ProblemDetails
            {
                Title = "UnExpected Error Accured.. We're Working on it.!",
                Detail = exception?.Message,
                Status = StatusCodes.Status500InternalServerError,
                Extensions = {
                    {"traceId",Activity.Current?.TraceId}
                }
            };

            var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                problem.Detail = exception?.Message;
                problem.Extensions.Add("stackTrace", exception?.StackTrace);
            }
            await Results.Problem(problem).ExecuteAsync(context);
        });
    }
}