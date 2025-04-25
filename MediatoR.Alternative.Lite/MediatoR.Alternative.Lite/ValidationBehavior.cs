using FluentResults;
using FluentValidation;
using Shared.Results.Errors;

namespace Shared.MediatorAlt
{
    /// <summary>
    /// Validation pipeline behavior that executes request validators
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes validation behavior
        /// </summary>
        /// <param name="validators">Registered validators for the request type</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Validates the request before executing the handler
        /// </summary>
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<Result<TResponse>>> next)
        {
            // Short-circuit if no validators registered
            if (!_validators.Any())
                return await next();

            // Execute all validators in parallel
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Aggregate validation errors
            var errors = validationResults
                .SelectMany(r => r.Errors)
                .Select(e => new ValidationFieldError(
                    message: e.ErrorMessage,
                    errorCode: e.ErrorCode,
                    propertyName: e.PropertyName,
                    attemptedValue: e.AttemptedValue))
                .ToList();

            // Return validation failure if errors exist
            if (errors.Count == 0)
                return await next();

            return Result.Fail<TResponse>(new ValidationError(request.GetType().FullName, errors));
        }
    }
}