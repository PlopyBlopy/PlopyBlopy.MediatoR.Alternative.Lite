using FluentResults;

namespace Shared.MediatorAlt
{
    /// <summary>
    /// Represents a service for sending requests and handling the execution pipeline
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Executes a request and returns a wrapped response with error handling
        /// </summary>
        /// <typeparam name="TResponse">The expected response type</typeparam>
        /// <param name="request">The request to execute (command/query)</param>
        /// <param name="ct">Cancellation token for aborting the operation</param>
        /// <returns>
        /// Task containing a Result wrapper with either:
        /// - Success: Contains the response data
        /// - Error: Contains validation or execution errors
        /// </returns>
        /// <remarks>
        /// Implementations should:
        /// 1. Find the appropriate request handler
        /// 2. Execute any registered pipeline behaviors
        /// 3. Handle errors and wrap results consistently
        /// </remarks>
        Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    }
}