using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ClientValidation
    {
        // Nombre / Apellido: una sola palabra, solo letras (con tildes/ñ), no puntos ni especiales
        public static bool IsValidSingleWordName(string? s) =>
            TextRules.IsSingleWordLettersOnly(s) && TextRules.MinLen(s, 3) && TextRules.MaxLen(s, 15);

        // Segundo nombre opcional con la misma regla
        public static bool IsValidOptionalSingleWord(string? s) =>
            string.IsNullOrWhiteSpace(s) || IsValidSingleWordName(s);

        // Email: validación estándar de EmailAddress (se usa en UI), aquí solo límite de largo/normalización
        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.MaxLen(s, 100);

        // Teléfono: exactamente 8 dígitos, solo números
        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsDigitsOnly(s) && TextRules.LenEquals(s, 8);

        // Dirección: palabras con una separación, se permiten puntos, sin otros especiales; largo máx 50
        public static bool IsValidAddress(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return true; // opcional
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersSpacesAndDotsOnly(n)) return false;
            // No letras sueltas (>=2)
            if (!TextRules.IsWordsMin2SingleSpacesAllowTrailingDot(n)) return false;
            return TextRules.MaxLen(n, 50);
        }

        public static void Normalize(Client c)
        {
            c.FirstName = TextRules.CanonicalTitle(c.FirstName);
            c.LastName  = TextRules.CanonicalTitle(c.LastName);
            if (!string.IsNullOrWhiteSpace(c.MiddleName))
                c.MiddleName = TextRules.CanonicalTitle(c.MiddleName);

            if (!string.IsNullOrWhiteSpace(c.Address))
                c.Address = TextRules.CanonicalTitle(c.Address);
        }

        public static IEnumerable<ValidationError> Validate(Client c)
        {
            if (!IsValidSingleWordName(c.FirstName))
                yield return new ValidationError(nameof(c.FirstName),
                    "Nombre inválido. Solo letras, 1 palabra, mín. 3.");

            if (!IsValidSingleWordName(c.LastName))
                yield return new ValidationError(nameof(c.LastName),
                    "Apellido inválido. Solo letras, 1 palabra, mín. 3.");

            if (!IsValidOptionalSingleWord(c.MiddleName))
                yield return new ValidationError(nameof(c.MiddleName),
                    "Segundo nombre inválido. Solo letras, 1 palabra, mín. 3.");

            if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
                yield return new ValidationError(nameof(c.Email),
                    "Correo inválido.");

            if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
                yield return new ValidationError(nameof(c.Phone),
                    "Teléfono inválido. Debe tener 8 dígitos.");

            if (!string.IsNullOrWhiteSpace(c.Address) && !IsValidAddress(c.Address))
                yield return new ValidationError(nameof(c.Address),
                    "Dirección inválida. Solo letras/espacios/puntos; 1 espacio entre palabras.");
        }
    }
}
