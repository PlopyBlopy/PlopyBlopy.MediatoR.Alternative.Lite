namespace Shared.MediatorAlt
{
    /// <summary>
    /// Marker interface for requests that return a specific response type
    /// </summary>
    /// <typeparam name="TResponse">Expected response type for the request</typeparam>
    public interface IRequest<TResponse>
    {
    }
}