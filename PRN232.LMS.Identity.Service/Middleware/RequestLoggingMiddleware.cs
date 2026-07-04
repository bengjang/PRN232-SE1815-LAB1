using System.Diagnostics;
using Serilog.Context;

namespace PRN232.LMS.Identity.Service.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";

        using (LogContext.PushProperty("RequestPath", path))
        using (LogContext.PushProperty("HttpMethod", method))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
                using (LogContext.PushProperty("ExecutionTimeMs", stopwatch.ElapsedMilliseconds))
                {
                    logger.LogInformation(
                        "HTTP {HttpMethod} {RequestPath} => {StatusCode} in {ExecutionTimeMs}ms",
                        method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
