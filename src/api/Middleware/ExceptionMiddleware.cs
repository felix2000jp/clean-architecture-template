using core.Results;
using Microsoft.AspNetCore.Mvc;

namespace api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var apiError = ResultTypes.InternalServerError("Sorry, an error occurred");
            _logger.LogWarning("An error occurred: {errorMessage} {@apiError}", apiError.GetMessage(), apiError);
            _logger.LogError(exception, "{exceptionMessage}", exception.Message);

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