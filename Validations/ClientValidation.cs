using BookstoreManagementSystem.Models;
using System.Collections.Generic;


namespace BookstoreManagementSystem.Validations
{
    public static class ClientValidation
    {
        // Nombre: solo letras minúsculas, sin espacios, sin números, max 15
        public static bool IsValidName(string? s) =>
            TextRules.IsLowercaseLettersNoSpaces(s) &&
            TextRules.MinLen(s, 1) &&
            TextRules.MaxLen(s, 15);

        // Segundo nombre opcional con las mismas reglas que nombre
        public static bool IsValidOptionalName(string? s) =>
            string.IsNullOrWhiteSpace(s) || IsValidName(s);

        // Email: sin espacios, debe contener '@' y terminar en .com, max 30
        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) &&
            TextRules.IsValidEmailNoSpacesAndCom(s) &&
            TextRules.MaxLen(s, 30);

        // Teléfono: sólo números, sin espacios, max 15
        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) &&
            TextRules.IsDigitsOnly(s) &&
            TextRules.MaxLen(s, 15);

        // Dirección: puede tener espacios, máximo 50 caracteres
        public static bool IsValidAddress(string? s) => TextRules.MaxLen(s, 50);

        // Validacion de la entidad
        public static IEnumerable<ValidationError> Validate(Client c)
        {
            if (!IsValidName(c.FirstName))
                yield return new ValidationError(nameof(c.FirstName), "Nombre invalido (solo letras minúsculas, sin espacios, max 15).");

            if (!IsValidName(c.LastName))
                yield return new ValidationError(nameof(c.LastName), "Apellido invalido (solo letras minúsculas, sin espacios, max 15).");

            if (!IsValidOptionalName(c.MiddleName))
                yield return new ValidationError(nameof(c.MiddleName), "Segundo nombre invalido (solo letras minúsculas, sin espacios, max 15).");

            if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
                yield return new ValidationError(nameof(c.Email), "Email invalido (sin espacios, debe contener @ y terminar en .com, max 30).");

            if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
                yield return new ValidationError(nameof(c.Phone), "Telefono invalido (solo numeros, sin espacios, max 15).");

            if (!string.IsNullOrWhiteSpace(c.Address) && !IsValidAddress(c.Address))
                yield return new ValidationError(nameof(c.Address), "La dirección es demasiado larga (max. 50).");
        }
    }
}
