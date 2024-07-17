using core.Results;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace api.Middlewares;

public class ExceptionMiddleware(ILogger logger) : IMiddleware
{
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var errorResult = ResultTypes.InternalServerError("Unexpected server error");
            _logger
                .ForContext("errorResult", errorResult, true)
                .Warning("An error occurred: {errorMessage}", errorResult.GetMessage());

            _logger.Error(exception, "An exception occurred: {exceptionMessage}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Title = errorResult.Title,
                Detail = errorResult.Detail,
                Status = (int)errorResult.Status,
                Extensions = new Dictionary<string, object?> { { "validationErrors", errorResult.ValidationErrors } }
            };

            context.Response.StatusCode = (int)problemDetails.Status;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}