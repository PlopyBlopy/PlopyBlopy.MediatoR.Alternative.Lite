namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// # Token Interface for Command Operations (CQRS)
    /// # Responsibilities:
    /// # - Indicates a state change operation
    /// # - May contain business logic
    /// # Type Parameters:
    /// # - TResponse: Type of result returned
    /// # Related Abstractions:
    /// #   - IRequestHandler<TRequest, TResponse>
    /// #   - IRequest<TResponse>
    /// # Examples:
    /// #   - CreateUserCommand → UserCreationResult
    /// #   - UpdateOrderCommand → OrderUpdateStatus
    /// </summary>
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}