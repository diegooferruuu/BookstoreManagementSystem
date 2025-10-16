using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ClientValidation
    {
        // Nombre / Apellido: letras y espacios simples; cada palabra >= 2; sin dígitos; máx 15
        public static bool IsValidName(string? s) =>
            TextRules.IsWordsLettersWithSingleSpacesMin2(s) &&
            TextRules.HasNoDigits(s) &&
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

        public static void Normalize(Client c)
        {
            c.FirstName = TextRules.CanonicalTitle(c.FirstName);
            c.LastName  = TextRules.CanonicalTitle(c.LastName);
            if (!string.IsNullOrWhiteSpace(c.MiddleName))
                c.MiddleName = TextRules.CanonicalTitle(c.MiddleName);

            if (!string.IsNullOrWhiteSpace(c.Address))
                c.Address = TextRules.NormalizeSpaces(c.Address);
        }

        // Validación de la entidad
        public static IEnumerable<ValidationError> Validate(Client c)
        {
            if (!IsValidName(c.FirstName))
                yield return new ValidationError(nameof(c.FirstName),
                    "Nombre inválido: solo letras y espacios simples; sin palabras de 1 letra ni números. Máx. 15.");

            if (!IsValidName(c.LastName))
                yield return new ValidationError(nameof(c.LastName),
                    "Apellido inválido: solo letras y espacios simples; sin palabras de 1 letra ni números. Máx. 15.");

            if (!IsValidOptionalName(c.MiddleName))
                yield return new ValidationError(nameof(c.MiddleName),
                    "Segundo nombre inválido (mismas reglas que nombre, máx. 15).");

            if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
                yield return new ValidationError(nameof(c.Email),
                    "Email inválido (sin espacios, debe contener @ y terminar en .com, máx. 30).");

            if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
                yield return new ValidationError(nameof(c.Phone),
                    "Teléfono inválido (solo números, sin espacios, máx. 15).");

            if (!string.IsNullOrWhiteSpace(c.Address) && !IsValidAddress(c.Address))
                yield return new ValidationError(nameof(c.Address),
                    "La dirección es demasiado larga (máx. 50).");
        }
    }
}
