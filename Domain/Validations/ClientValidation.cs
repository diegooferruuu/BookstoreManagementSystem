using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ClientValidation
    {
        public static bool IsValidSingleWordName(string? s) =>
            TextRules.IsSingleWordLettersOnly(s) && TextRules.MinLen(s, 3) && TextRules.MaxLen(s, 15);

        public static bool IsValidOptionalSingleWord(string? s) =>
            string.IsNullOrWhiteSpace(s) || IsValidSingleWordName(s);

        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.MaxLen(s, 100);

        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsDigitsOnly(s) && TextRules.LenEquals(s, 8);

        public static bool IsValidAddress(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return true;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsValidCbbaAddress(n)) return false;
            return TextRules.MaxLen(n, 60);
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
                    "Nombre inválido. Solo letras. Una palabra. Mínimo 3 letras.");

            if (!(TextRules.IsCompoundLastName(c.LastName) || IsValidSingleWordName(c.LastName)))
                yield return new ValidationError(nameof(c.LastName),
                    "Apellido inválido. Solo letras. Acepta compuesto con partículas. Mínimo 3 en partes principales.");

            if (!IsValidOptionalSingleWord(c.MiddleName))
                yield return new ValidationError(nameof(c.MiddleName),
                    "Segundo nombre inválido. Solo letras. Una palabra. Mínimo 3 letras.");

            if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
                yield return new ValidationError(nameof(c.Email),
                    "Correo inválido.");

            if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
                yield return new ValidationError(nameof(c.Phone),
                    "Teléfono inválido. Debe tener 8 dígitos.");

            if (!string.IsNullOrWhiteSpace(c.Address) && !IsValidAddress(c.Address))
                yield return new ValidationError(nameof(c.Address),
                    "Dirección inválida. Solo letras, números, espacios, punto y '/'.");
        }

        public static Results.Result ValidateAsResult(Client c)
            => Results.Result.FromValidation(Validate(c));

        public static BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Client> ValidateAndWrap(Client c)
        {
            var errors = Validate(c).ToList();
            return errors.Count == 0
                ? BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Client>.Ok(c)
                : BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Client>.FromErrors(errors);
        }
    }
}
