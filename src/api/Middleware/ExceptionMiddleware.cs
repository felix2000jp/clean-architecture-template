using core.Results;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace api.Middleware;

public class ExceptionMiddleware(ILogger logger, RequestDelegate next)
{
    private readonly ILogger _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.Error(
                exception,
                "An unexpected server error occurred: [{message}]",
                exception.Message);

            var apiError = ResultTypes.UnexpectedError();

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Title = apiError.Title,
                Detail = apiError.Detail,
                Status = (int)apiError.Status,
                Extensions = new Dictionary<string, object?> { { "validationErrors", apiError.ValidationErrors } }
            };

            context.Response.StatusCode = (int)problemDetails.Status;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}