using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Interfaces;
using System.Linq;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ProductValidation
    {
        private const int DescriptionMaxLength = 120;
        private const int NameMaxLength = 80;

        public const decimal MaxPrice = 9999m;

        public static bool IsValidName(string? s)
        {
            return !TextRules.GetProductNameErrors(s).Any();
        }

        public static bool IsValidCategory_id(Guid s, ICategoryRepository categoryRepository)
        {
            if (s == Guid.Empty) return false;
            var category = categoryRepository.Read(s);
            return category != null;
        }

        public static bool IsValidDescriptionContent(string? s)
        {
            return TextRules.IsValidProductDescriptionLoose(s);
        }

        public static bool IsValidPrice(decimal? price) => price.HasValue && price.Value > 0m && price.Value <= MaxPrice;

        public static bool IsValidStock(int? stock) => stock.HasValue && stock.Value >= 0;

        public static void Normalize(Product p)
        {
            p.Name = TextRules.CanonicalProductName(p.Name);
            if (!string.IsNullOrWhiteSpace(p.Description))
                p.Description = TextRules.CanonicalSentence(p.Description);
        }

        public static IEnumerable<ValidationError> Validate(Product p, ICategoryRepository categoryRepository)
        {
            var nName = TextRules.NormalizeSpaces(p.Name);
            if (!string.IsNullOrEmpty(nName) && nName.Length > NameMaxLength)
            {
                yield return new ValidationError(nameof(p.Name), $"El nombre no debe superar {NameMaxLength} caracteres.");
            }
            else
            {
                var nameErrors = TextRules.GetProductNameErrors(p.Name).ToList();
                foreach (var msg in nameErrors)
                    yield return new ValidationError(nameof(p.Name), msg);
            }

            if (!IsValidCategory_id(p.Category_id, categoryRepository))
                yield return new ValidationError(nameof(p.Category_id),
                    "Categoría inválida.");

            var descNorm = TextRules.NormalizeSpaces(p.Description);
            if (!string.IsNullOrEmpty(descNorm) && descNorm.Length > DescriptionMaxLength)
            {
                yield return new ValidationError(nameof(p.Description),
                    $"La descripción no debe superar {DescriptionMaxLength} caracteres.");
            }
            else if (!IsValidDescriptionContent(p.Description))
            {
                yield return new ValidationError(nameof(p.Description),
                    "Descripción inválida. Letras, números, espacios, puntos y comas.");
            }

            if (p.Price <= 0)
            {
                yield return new ValidationError(nameof(p.Price),
                    "Precio inválido. Debe ser mayor a 0.");
            }
            else if (p.Price > MaxPrice)
            {
                yield return new ValidationError(nameof(p.Price),
                    $"El precio no debe superar {MaxPrice}.");
            }

            if (!IsValidStock(p.Stock))
                yield return new ValidationError(nameof(p.Stock),
                    "Stock inválido. No puede ser negativo.");
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
