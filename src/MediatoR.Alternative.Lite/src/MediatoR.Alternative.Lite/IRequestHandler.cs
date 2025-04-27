using FluentResults;

namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Handles execution of a specific request type
    /// </summary>
    /// <typeparam name="TRequest">Type of request being handled</typeparam>
    /// <typeparam name="TResponse">Type of response returned</typeparam>
    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Processes the request and returns a wrapped result
        /// </summary>
        /// <param name="request">Request instance to handle</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>
        /// Task containing Result with either:
        /// - Success: Valid response data
        /// - Error: Domain or infrastructure failure
        /// </returns>
        Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct);
    }
}