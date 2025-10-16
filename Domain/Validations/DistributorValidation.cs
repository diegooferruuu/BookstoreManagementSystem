using System.Collections.Generic;
using System.Linq;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class DistributorValidation
    {
        // Nombre: varias palabras, solo letras (con tildes/ñ) y espacios simples; cada palabra >= 3; sin dígitos ni especiales; máx 20
        public static bool IsValidName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersAndSpaces(n)) return false;
            var parts = n.Split(' ');
            if (parts.Any(p => p.Length < 3)) return false;
            return TextRules.MaxLen(n, 20);
        }

        // Email: validación estándar de correo
        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsValidEmail(s) && TextRules.MaxLen(s, 100);

        // Teléfono: exactamente 8 dígitos, solo números
        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsDigitsOnly(s) && TextRules.LenEquals(s, 8);

        // Dirección: igual que cliente (letras, espacios y puntos; palabras separadas por un solo espacio); máx 50
        public static bool IsValidAddress(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return true;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersSpacesAndDotsOnly(n)) return false;
            if (!TextRules.IsWordsMin2SingleSpacesAllowTrailingDot(n)) return false;
            return TextRules.MaxLen(n, 50);
        }

        public static void Normalize(Distributor d)
        {
            d.Name = TextRules.CanonicalTitle(d.Name);
            if (!string.IsNullOrWhiteSpace(d.Address))
                d.Address = TextRules.CanonicalTitle(d.Address);
        }

        public static IEnumerable<ValidationError> Validate(Distributor d)
        {
            if (!IsValidName(d.Name))
                yield return new ValidationError(nameof(d.Name),
                    "Nombre inválido. Solo letras y espacios; 3+ letras por palabra.");

            if (!IsValidEmail(d.ContactEmail))
                yield return new ValidationError(nameof(d.ContactEmail),
                    "Correo inválido.");

            if (!IsValidPhone(d.Phone))
                yield return new ValidationError(nameof(d.Phone),
                    "Teléfono inválido. Debe tener 8 dígitos.");

            if (!IsValidAddress(d.Address))
                yield return new ValidationError(nameof(d.Address),
                    "Dirección inválida. Solo letras/espacios/puntos; 1 espacio entre palabras.");
        }
    }
}
