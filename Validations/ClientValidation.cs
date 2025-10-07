using BookstoreManagementSystem.Models;
using System.Collections.Generic;


namespace BookstoreManagementSystem.Validations
{
    public static class ClientValidation
    {
        public static bool IsValidName(string? s) =>
            TextRules.IsLettersAndSpaces(s) &&
            TextRules.MinLen(s, 2) &&
            TextRules.MaxLen(s, 20) &&
            TextRules.NotAllUppercase(s);

        public static bool IsValidOptionalName(string? s) =>
            string.IsNullOrWhiteSpace(s) || IsValidName(s);

        public static bool IsValidEmail(string? s) => TextRules.IsValidEmail(s);
        public static bool IsValidPhone(string? s) => TextRules.IsValidPhone(s);
        public static bool IsValidAddress(string? s) => TextRules.MaxLen(s, 50);

        // Validacion de la entidad
        public static IEnumerable<ValidationError> Validate(Client c)
        {
            if (!IsValidName(c.FirstName))
                yield return new ValidationError(nameof(c.FirstName), "Nombre invalido (solo letras/espacios, 2–20, no todo en MAYUSCULAS).");

            if (!IsValidName(c.LastName))
                yield return new ValidationError(nameof(c.LastName), "Apellido invalido (solo letras/espacios, 2–20, no todo en MAYUSCULAS).");

            if (!IsValidOptionalName(c.MiddleName))
                yield return new ValidationError(nameof(c.MiddleName), "Segundo nombre invalido.");

            if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
                yield return new ValidationError(nameof(c.Email), "Email invalido.");

            if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
                yield return new ValidationError(nameof(c.Phone), "Telefono invalido.");

            if (!string.IsNullOrWhiteSpace(c.Address) && !IsValidAddress(c.Address))
                yield return new ValidationError(nameof(c.Address), "La dirección es demasiado larga (max. 250).");
        }
    }
}
