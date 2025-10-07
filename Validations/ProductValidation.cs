using System.Collections.Generic;
using BookstoreManagementSystem.Models;

namespace BookstoreManagementSystem.Validations
{
    public static class ProductValidation
    {
        public static bool IsValidName(string? s) =>
            !string.IsNullOrWhiteSpace(s) &&
            TextRules.MinLen(s, 2) &&
            TextRules.MaxLen(s, 20) &&
            TextRules.NotAllUppercase(s);

        public static bool IsValidCategory_id(int s, CategoryRepository categoryRepository)
        {
            if(categoryRepository.Read(s) != null)
                return true;
            return false;
        }

        public static bool IsValidDescription(string? s) =>
            string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 500);

        public static bool IsValidPrice(decimal price) => TextRules.IsNonNegative(price);
        public static bool IsValidStock(int stock) => TextRules.IsNonNegativeInt(stock);

        public static IEnumerable<ValidationError> Validate(Product p, CategoryRepository categoryRepository)
        {
            if (!IsValidName(p.Name))
                yield return new ValidationError(nameof(p.Name), "Nombre de producto invalido (2–250, no todo en MAYUSCULAS).");

            if (!IsValidCategory_id(p.Category_id.Value, categoryRepository))
                yield return new ValidationError(nameof(p.Category_id), "Categoria invalida, no se encuentra entre las disponibles.");

            if (!IsValidDescription(p.Description))
                yield return new ValidationError(nameof(p.Description), "Descripcion demasiado larga (max. 500).");

            if (!IsValidPrice(p.Price))
                yield return new ValidationError(nameof(p.Price), "El precio debe ser >= 0.");

            if (!IsValidStock(p.Stock))
                yield return new ValidationError(nameof(p.Stock), "El stock debe ser >= 0.");
        }
    }
}