using ServiceCommon.Domain.Validations;

namespace ServiceCommon.Application.Services
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationError> errors)
            : base("Validation failed")
        {
            Errors = errors.ToList();
        }

        public ValidationException(params ValidationError[] errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
