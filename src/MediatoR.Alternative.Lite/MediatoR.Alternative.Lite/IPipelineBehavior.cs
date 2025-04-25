using FluentResults;

namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Base interface for pipeline behaviors (middleware)
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Pipeline handler method
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Next delegate in pipeline</param>
        /// <returns>Final processed response</returns>
        Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<Result<TResponse>>> next);
    }
}