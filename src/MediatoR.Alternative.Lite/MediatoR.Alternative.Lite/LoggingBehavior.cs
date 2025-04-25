using FluentResults;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Example pipeline behavior for request logging
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Basic logging implementation that just continues the pipeline
        /// </summary>
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<Result<TResponse>>> next)
        {
            const string behaviorName = nameof(LoggingBehavior<TRequest, TResponse>);

            try
            {
                // Log request start
                _logger.LogInformation("[{Behavior}] Handling {RequestType}: {@Request}",
                    behaviorName, typeof(TRequest).Name, request);

                var stopwatch = Stopwatch.StartNew();

                // Continue pipeline
                var response = await next();

                stopwatch.Stop();

                // Log successful completion
                _logger.LogInformation("[{Behavior}] Completed {RequestType} in {ElapsedMilliseconds}ms",
                    behaviorName, typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                // Log errors
                _logger.LogError(ex, "[{Behavior}] Failed to handle {RequestType}",
                    behaviorName, typeof(TRequest).Name);
                throw;
            }
        }
    }
}