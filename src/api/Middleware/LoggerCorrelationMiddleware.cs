using api.Configuration;
using Serilog.Context;

namespace api.Middleware;

public class LoggerCorrelationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var correlationIdHeaderValue);
        var correlationId = correlationIdHeaderValue.FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Response.Headers[HeaderNames.CorrelationId] = correlationId;

        using (LogContext.PushProperty(HeaderNames.CorrelationId, correlationId))
        {
            await _next(context);
        }
    }
}