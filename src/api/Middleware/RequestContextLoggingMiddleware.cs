using Serilog.Context;

namespace api.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationHeaderValue);
        var correlationId = correlationHeaderValue.FirstOrDefault() ?? context.TraceIdentifier;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}