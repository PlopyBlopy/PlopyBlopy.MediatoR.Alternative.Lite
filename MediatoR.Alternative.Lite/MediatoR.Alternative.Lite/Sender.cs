using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.MediatorAlt
{
    public class Sender : ISender
    {
        private readonly IServiceProvider _serviceProvider;

        public Sender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends a request through the handler pipeline
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Result wrapped response</returns>
        public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        {
            // Get concrete request and response types
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            // Create generic handler type (IRequestHandler<TRequest, TResponse>)
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            // Resolve handler dynamically from service provider
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            // Execute the request processing pipeline
            return await SendPipeline(request, handler, requestType, responseType, ct);
        }

        /// <summary>
        /// Builds and executes the processing pipeline with registered behaviors
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="handler">Resolved request handler</param>
        /// <param name="requestType">Type of the request</param>
        /// <param name="responseType">Type of the response</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Result wrapped response</returns>
        private async Task<Result<TResponse>> SendPipeline<TResponse>(IRequest<TResponse> request, object handler, Type requestType, Type responseType, CancellationToken ct = default)
        {
            // Get handler's Handle method using reflection
            var handleMethod = handler.GetType().GetMethod("Handle");

            // Create base pipeline: direct handler execution
            Func<Task<Result<TResponse>>> pipeline = async () =>
            {
                var result = (Task<Result<TResponse>>)handleMethod.Invoke(handler, new object[] { request, ct });
                return await result;
            };

            // Resolve all pipeline behaviors in reverse order (outermost behavior first)
            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            var behaviors = _serviceProvider.GetServices(behaviorType)
                .Reverse()
                .ToList();

            // Wrap each behavior around the current pipeline
            foreach (var behavior in behaviors)
            {
                var currentBehavior = behavior;
                var next = pipeline;

                // Create new pipeline step that includes current behavior
                pipeline = async () =>
                {
                    // Get behavior's Handle method using reflection
                    var handleBehaviorMethod = currentBehavior.GetType().GetMethod("Handle");

                    // Execute behavior with next pipeline step
                    var result = (Task<Result<TResponse>>)handleBehaviorMethod.Invoke(
                        currentBehavior,
                        new object[] { request, ct, next });
                    return await result;
                };
            }

            // Execute the full pipeline
            return await pipeline();
        }
    }
}