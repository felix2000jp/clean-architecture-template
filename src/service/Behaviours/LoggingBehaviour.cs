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
        _logger.Information("Starting {request}", typeof(TRequest).Name);

        var result = await next();

        if (result.IsError)
        {
            _logger.Error("Occurred error [{@error}]", result.Error);
        }

        _logger.Information("Finished {request}", typeof(TRequest).Name);

        return result;
    }
}