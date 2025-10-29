using System.Collections.Generic;
using ServiceProducts.Domain.Models;
using ServiceProducts.Domain.Interfaces;
using ServiceCommon.Domain.Validations;
using System.Linq;

namespace ServiceProducts.Domain.Validations
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

        public static bool IsValidCategoryId(Guid s, ICategoryRepository categoryRepository)
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

            if (!IsValidCategoryId(p.CategoryId, categoryRepository))
                yield return new ValidationError(nameof(p.CategoryId),
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

        public static ServiceCommon.Domain.Results.Result ValidateAsResult(Product p, ICategoryRepository categoryRepository)
            => ServiceCommon.Domain.Results.Result.FromValidation(Validate(p, categoryRepository));

        public static ServiceCommon.Domain.Results.Result<Product> ValidateAndWrap(Product p, ICategoryRepository categoryRepository)
        {
            var errors = Validate(p, categoryRepository).ToList();
            return errors.Count == 0
                ? ServiceCommon.Domain.Results.Result<Product>.Ok(p)
                : ServiceCommon.Domain.Results.Result<Product>.FromErrors(errors);
        }
    }
}
