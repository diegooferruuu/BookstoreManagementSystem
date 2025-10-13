using System.Collections.Generic;
using BookstoreManagementSystem.Models;

namespace BookstoreManagementSystem.Validations
{
    public static class ProductValidation
    {
        // nombre: solo letras minúsculas, con espacios, max 20
        public static bool IsValidName(string? s) =>
            TextRules.IsLowercaseLetters(s) &&
            TextRules.MinLen(s, 1) &&
            TextRules.MaxLen(s, 20);

        // Categoria requerida: debe existir en el repositorio
        public static bool IsValidCategory_id(int? s, CategoryRepository categoryRepository)
        {
            if (!s.HasValue) return false;
            return categoryRepository.Read(s.Value) != null;
        }

        // descripcion: acepta caracteres especiales, max 80
        public static bool IsValidDescription(string? s) =>
            string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 80);

    // precio: debe ser numero > 0
    public static bool IsValidPrice(decimal? price) => price.HasValue && price.Value > 0m;

    // Stock: entero >= 0 (puede ser 0)
    public static bool IsValidStock(int? stock) => stock.HasValue && stock.Value >= 0;

        public static IEnumerable<ValidationError> Validate(Product p, CategoryRepository categoryRepository)
        {
            if (!IsValidName(p.Name))
                yield return new ValidationError(nameof(p.Name), "Nombre de producto invalido (solo letras minusculas, sin espacios, max 20).");

            if (!IsValidCategory_id(p.Category_id, categoryRepository))
                yield return new ValidationError(nameof(p.Category_id), "Categoria invalida, debe seleccionar una categoria valida.");

            if (!IsValidDescription(p.Description))
                yield return new ValidationError(nameof(p.Description), "Descripcion demasiado larga (max. 80).");

            if (!IsValidPrice(p.Price))
                yield return new ValidationError(nameof(p.Price), "El precio debe ser un numero mayor que 0.");

            if (!IsValidStock(p.Stock))
                yield return new ValidationError(nameof(p.Stock), "El stock debe ser un entero >= 0 (puede ser 0).");
        }
    }
}