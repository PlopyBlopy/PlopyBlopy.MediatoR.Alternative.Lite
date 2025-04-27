using FluentResults;

namespace MediatoR.Alternative.Lite.Errors
{
    public sealed class ValidationFieldError : IError
    {
        public List<IError>? Reasons => null;
        public string Message { get; }
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        public ValidationFieldError(string message, string errorCode, string propertyName, object attemptedValue)
        {
            Message = message;
            Metadata.Add("errorCode", errorCode);
            Metadata.Add("propertyName", propertyName);
            Metadata.Add("attemptedValue", attemptedValue);
        }
    }
}
