using FluentResults;

namespace Shared.Results.Errors
{
    public sealed class ValidationError : IError
    {
        public List<IError> Reasons { get; set; } = new List<IError>();
        public string Message { get; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        public ValidationError(string errorCode, List<ValidationFieldError> fieldErrors)
        {
            Message = "Validation error.";
            Metadata.Add("errorCode", errorCode);
            Reasons.AddRange(fieldErrors);
        }
    }
}