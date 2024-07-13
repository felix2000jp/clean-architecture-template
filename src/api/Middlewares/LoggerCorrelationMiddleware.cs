using api.Configurations;
using Serilog.Context;

namespace api.Middlewares;

public class LoggerCorrelationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CustomHeaders.CorrelationId, out var headerValue);
        var correlationId = headerValue.FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Request.Headers[CustomHeaders.CorrelationId] = correlationId;
        context.Response.Headers[CustomHeaders.CorrelationId] = correlationId;

        using (LogContext.PushProperty("correlationId", correlationId))
        {
            await _next(context);
        }
    }
}