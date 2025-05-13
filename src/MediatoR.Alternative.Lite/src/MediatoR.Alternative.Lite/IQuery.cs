namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// # Token Interface for Query Operations (CQRS)
    /// # Responsibilities:
    /// # - Indicates a data read operation
    /// # - Should not change the state of the system
    /// # Type Parameters:
    /// #   - TResponse: Тип возвращаемого результата
    /// # Related Abstractions:
    /// #   - IRequestHandler<TRequest, TResponse>
    /// #   - IRequest<TResponse>
    /// </summary>
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}