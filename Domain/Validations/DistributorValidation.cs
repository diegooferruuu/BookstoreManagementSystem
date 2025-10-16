using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class DistributorValidation
    {
        // Nombre: letras y espacios simples; cada palabra >= 2; sin dígitos; máx 20
        public static bool IsValidName(string? s) =>
            TextRules.IsWordsLettersWithSingleSpacesMin2(s) &&
            TextRules.HasNoDigits(s) &&
            TextRules.MinLen(s, 1) &&
            TextRules.MaxLen(s, 20);

        // Email: obligatorio si provisto, sin espacios, debe contener @ y terminar en .com
        public static bool IsValidEmail(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : TextRules.IsValidEmailNoSpacesAndCom(s);

        // Teléfono: solo dígitos, sin espacios, máx 15
        public static bool IsValidPhone(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : TextRules.IsDigitsOnly(s) && TextRules.MaxLen(s, 15);

        // Dirección: puede tener caracteres especiales, máximo 50
        public static bool IsValidAddress(string? s) => string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 50);

        public static void Normalize(Distributor d)
        {
            d.Name = TextRules.CanonicalTitle(d.Name);
            if (!string.IsNullOrWhiteSpace(d.Address))
                d.Address = TextRules.NormalizeSpaces(d.Address);
        }

        public static IEnumerable<ValidationError> Validate(Distributor d)
        {
            if (!IsValidName(d.Name))
                yield return new ValidationError(nameof(d.Name),
                    "Nombre inválido: solo letras y espacios simples; sin palabras de 1 letra ni números. Máx. 20.");

            if (!string.IsNullOrWhiteSpace(d.ContactEmail) && !IsValidEmail(d.ContactEmail))
                yield return new ValidationError(nameof(d.ContactEmail),
                    "Email de contacto inválido (sin espacios, debe contener @ y terminar en .com).");

            if (!string.IsNullOrWhiteSpace(d.Phone) && !IsValidPhone(d.Phone))
                yield return new ValidationError(nameof(d.Phone),
                    "Teléfono inválido (solo números, sin espacios, máx. 15).");

            if (!IsValidAddress(d.Address))
                yield return new ValidationError(nameof(d.Address),
                    "Dirección demasiado larga (máx. 50).");
        }
    }
}
