using System.Collections.Generic;
using System.Linq;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Domain.Results
{
    public static class ValidationResultExtensions
    {
        public static Result ToResult(this IEnumerable<ValidationError> errors)
            => Result.FromErrors(errors);

        public static Result<T> ToResult<T>(this IEnumerable<ValidationError> errors, T value)
            => (errors == null || !errors.Any())
                ? Result<T>.Ok(value)
                : Result<T>.FromErrors(errors);
    }
}
