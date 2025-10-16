using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ProductValidation
    {
        // Nombre: solo letras (con acentos) y espacios simples; cada palabra >= 2 letras; max 20
        public static bool IsValidName(string? s) =>
            TextRules.IsWordsLettersWithSingleSpacesMin2(s) &&
            TextRules.HasNoDigits(s) &&
            TextRules.MinLen(s, 1) &&
            TextRules.MaxLen(s, 20);

        // Categoria requerida: debe existir en el repositorio
        public static bool IsValidCategory_id(Guid s, ICategoryRepository categoryRepository)
        {
            if (s == Guid.Empty) return false;
            var category = categoryRepository.Read(s);
            return category != null;
        }

        // descripcion: acepta caracteres especiales, max 80
        public static bool IsValidDescription(string? s) =>
            string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 80);

        // precio: > 0
        public static bool IsValidPrice(decimal? price) => price.HasValue && price.Value > 0m;

        // Stock: entero >= 0 (puede ser 0)
        public static bool IsValidStock(int? stock) => stock.HasValue && stock.Value >= 0;

        /// <summary>
        /// Normaliza campos para persistencia (Título y espacios).
        /// Llamar antes de guardar: ProductValidation.Normalize(p);
        /// </summary>
        public static void Normalize(Product p)
        {
            p.Name = TextRules.CanonicalTitle(p.Name);
            if (!string.IsNullOrWhiteSpace(p.Description))
                p.Description = TextRules.NormalizeSpaces(p.Description);
        }

        public static IEnumerable<ValidationError> Validate(Product p, ICategoryRepository categoryRepository)
        {
            if (!IsValidName(p.Name))
                yield return new ValidationError(nameof(p.Name),
                    "Nombre inválido: use solo letras (tildes/ñ) con espacios simples; nada de 'c o c a'. Máx. 20 caracteres.");

            if (!IsValidCategory_id(p.Category_id, categoryRepository))
                yield return new ValidationError(nameof(p.Category_id),
                    "Categoría inválida, seleccione una categoría válida.");

            if (!IsValidDescription(p.Description))
                yield return new ValidationError(nameof(p.Description),
                    "Descripción demasiado larga (máx. 80).");

            if (!IsValidPrice(p.Price))
                yield return new ValidationError(nameof(p.Price),
                    "El precio debe ser un número mayor que 0.");

            if (!IsValidStock(p.Stock))
                yield return new ValidationError(nameof(p.Stock),
                    "El stock debe ser un entero >= 0 (puede ser 0).");
        }
    }
}
