using core.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace service.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {requestPipeline}", typeof(TRequest).Name);

        var result = await next();

        if (result.IsError)
        {
            var apiError = result.Error;
            _logger.LogWarning("An error occurred: {errorMessage} {@apiError}", apiError.GetMessage(), apiError);
        }

        _logger.LogInformation("Finished {requestPipeline}", typeof(TRequest).Name);

        return result;
    }
}