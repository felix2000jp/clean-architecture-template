using core.Results;
using MediatR;
using Serilog;

namespace service.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    private readonly ILogger _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.Information("Starting {requestPipeline}", typeof(TRequest).Name);

        var result = await next();

        if (result.IsError)
        {
            var errorResult = result.Error;
            _logger
                .ForContext("errorResult", errorResult, true)
                .Warning("An error occurred: {errorMessage}", errorResult.GetMessage());
        }

        _logger.Information("Finished {requestPipeline}", typeof(TRequest).Name);

        return result;
    }
}