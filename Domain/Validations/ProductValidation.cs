using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Interfaces;
using System.Linq;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ProductValidation
    {
        public static bool IsValidName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersAndSpaces(n)) return false;
            if (!TextRules.HasNoSpacedOutLetters(n)) return false;
            var parts = n.Split(' ');
            if (parts.Any(p => p.Length < 3)) return false;
            return TextRules.MinLen(n, 1) && TextRules.MaxLen(n, 20);
        }

        public static bool IsValidCategory_id(Guid s, ICategoryRepository categoryRepository)
        {
            if (s == Guid.Empty) return false;
            var category = categoryRepository.Read(s);
            return category != null;
        }

        public static bool IsValidDescription(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersSpacesAndDotsOnly(n)) return false;
            if (!TextRules.IsWordsMin3SingleSpacesAllowTrailingDot(n)) return false;
            return TextRules.MaxLen(n, 80);
        }

        public static bool IsValidPrice(decimal? price) => price.HasValue && price.Value > 0m;

        public static bool IsValidStock(int? stock) => stock.HasValue && stock.Value >= 0;

        public static void Normalize(Product p)
        {
            p.Name = TextRules.CanonicalTitle(p.Name);
            if (!string.IsNullOrWhiteSpace(p.Description))
                p.Description = TextRules.CanonicalSentence(p.Description);
        }

        public static IEnumerable<ValidationError> Validate(Product p, ICategoryRepository categoryRepository)
        {
            if (!IsValidName(p.Name))
                yield return new ValidationError(nameof(p.Name),
                    "Nombre inválido. Solo letras y espacios, 3+ letras por palabra.");

            if (!IsValidCategory_id(p.Category_id, categoryRepository))
                yield return new ValidationError(nameof(p.Category_id),
                    "Seleccione una categoría válida.");

            if (!IsValidDescription(p.Description))
                yield return new ValidationError(nameof(p.Description),
                    "Descripción inválida. Solo letras, espacios y puntos. 3+ letras por palabra.");

            if (!IsValidPrice(p.Price))
                yield return new ValidationError(nameof(p.Price),
                    "El precio debe ser mayor que 0.");

            if (!IsValidStock(p.Stock))
                yield return new ValidationError(nameof(p.Stock),
                    "El stock no puede ser negativo.");
        }

        public static BookstoreManagementSystem.Domain.Results.Result ValidateAsResult(Product p, ICategoryRepository categoryRepository)
            => BookstoreManagementSystem.Domain.Results.Result.FromValidation(Validate(p, categoryRepository));

        public static BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Product> ValidateAndWrap(Product p, ICategoryRepository categoryRepository)
        {
            var errors = Validate(p, categoryRepository).ToList();
            return errors.Count == 0
                ? BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Product>.Ok(p)
                : BookstoreManagementSystem.Domain.Results.Result<BookstoreManagementSystem.Domain.Models.Product>.FromErrors(errors);
        }
    }
}
