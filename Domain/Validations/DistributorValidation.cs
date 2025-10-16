using System.Collections.Generic;
using System.Linq;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class DistributorValidation
    {
        public static bool IsValidName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersAndSpaces(n)) return false;
            var parts = n.Split(' ');
            if (parts.Any(p => p.Length < 3)) return false;
            return TextRules.MaxLen(n, 20);
        }

        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsValidEmail(s) && TextRules.MaxLen(s, 100);

        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsDigitsOnly(s) && TextRules.LenEquals(s, 8);

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

        public static BookstoreManagementSystem.Domain.Results.Result ValidateAsResult(Distributor d)
            => BookstoreManagementSystem.Domain.Results.Result.FromValidation(Validate(d));

        public static BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Distributor> ValidateAndWrap(Distributor d)
        {
            var errors = Validate(d).ToList();
            return errors.Count == 0
                ? BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Distributor>.Ok(d)
                : BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Distributor>.FromErrors(errors);
        }
    }
}
