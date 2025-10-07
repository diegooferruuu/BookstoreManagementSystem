using System.Collections.Generic;
using BookstoreManagementSystem.Models;

namespace BookstoreManagementSystem.Validations
{
    public static class DistributorValidation
    {
        public static bool IsValidName(string? s) =>
            TextRules.IsLettersAndSpaces(s) &&
            TextRules.MinLen(s, 2) &&
            TextRules.MaxLen(s, 20) &&
            TextRules.NotAllUppercase(s);

        public static bool IsValidEmail(string? s) => TextRules.IsValidEmail(s);
        public static bool IsValidPhone(string? s) => TextRules.IsValidPhone(s);
        public static bool IsValidAddress(string? s) => string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 50);

        public static IEnumerable<ValidationError> Validate(Distributor d)
        {
            if (!IsValidName(d.Name))
                yield return new ValidationError(nameof(d.Name), "Nombre invalido (solo letras/espacios, 2–20, no todo en MAYUSCULAS).");

            if (!string.IsNullOrWhiteSpace(d.ContactEmail) && !IsValidEmail(d.ContactEmail))
                yield return new ValidationError(nameof(d.ContactEmail), "Email de contacto invalido.");

            if (!string.IsNullOrWhiteSpace(d.Phone) && !IsValidPhone(d.Phone))
                yield return new ValidationError(nameof(d.Phone), "Telefono invalido.");

            if (!IsValidAddress(d.Address))
                yield return new ValidationError(nameof(d.Address), "Direccion demasiado larga (max. 250).");
        }
    }
}