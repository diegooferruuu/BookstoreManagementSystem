using System;
using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Application.Services
{
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationError> errors)
            : base("Validación fallida")
        {
            Errors = errors;
        }
    }
}
