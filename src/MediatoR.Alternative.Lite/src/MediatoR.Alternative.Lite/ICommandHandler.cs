namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Handles execution of a command request to modify state or perform actions
    /// </summary>
    /// <typeparam name="TRequest">Type of command request being handled, must implement ICommand<TResponse></typeparam>
    /// <typeparam name="TResponse">Type of response data returned from the command</typeparam>
    public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
    }
}