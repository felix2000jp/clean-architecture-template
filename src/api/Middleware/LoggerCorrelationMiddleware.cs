using Serilog.Context;

namespace api.Middleware;

public class LoggerCorrelationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationIdHeaderValue);
        var correlationId = correlationIdHeaderValue.FirstOrDefault() ?? Guid.NewGuid().ToString();

        using (LogContext.PushProperty("correlationId", correlationId))
        {
            await _next(context);
        }
    }
}