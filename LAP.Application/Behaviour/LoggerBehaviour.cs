using System.Diagnostics;
using LAP.Application.Interface;
using MediatR;

namespace LAP.Application.Behaviors;

/// <summary>Logs the start, completion and failure of every MediatR request with elapsed time.</summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICustomLogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.</summary>
    /// <param name="logger">The logger instance for recording request lifecycle events.</param>
    public LoggingBehavior(ICustomLogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>Executes the request pipeline while logging start, completion and any failures.</summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response from the downstream pipeline.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug("Handling request: {RequestName}", requestName);

        var response = await next();

        stopwatch.Stop();

        _logger.LogDebug(
            "Completed request: {RequestName} in {ElapsedMilliseconds} ms",
            requestName,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}
