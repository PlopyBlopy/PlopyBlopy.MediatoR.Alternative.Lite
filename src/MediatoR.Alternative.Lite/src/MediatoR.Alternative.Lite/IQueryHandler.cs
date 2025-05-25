namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Handles execution of a query request to retrieve data without side effects
    /// </summary>
    /// <typeparam name="TRequest">Type of query request being handled, must implement IQuery<TResponse></typeparam>
    /// <typeparam name="TResponse">Type of response data returned from the query</typeparam>
    public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
    }
}