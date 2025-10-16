using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Domain.Validations
{
    public static class ProductValidation
    {
        // Nombre: solo letras (con acentos) y espacios simples; cada palabra >= 3 letras; sin puntos ni caracteres especiales; máx. 20
        public static bool IsValidName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            // No permitir puntos ni caracteres especiales
            if (!TextRules.IsLettersAndSpaces(n)) return false;
            // Sin "c o c a" (palabras de 1 letra)
            if (!TextRules.HasNoSpacedOutLetters(n)) return false;
            // Cada palabra debe tener longitud mínima de 3
            var parts = n.Split(' ');
            if (parts.Any(p => p.Length < 3)) return false;
            return TextRules.MinLen(n, 1) && TextRules.MaxLen(n, 20);
        }

        // Categoria requerida: debe existir en el repositorio
        public static bool IsValidCategory_id(Guid s, ICategoryRepository categoryRepository)
        {
            if (s == Guid.Empty) return false;
            var category = categoryRepository.Read(s);
            return category != null;
        }

        // Descripción: obligatoria; permite solo letras, espacios y puntos; sin otros caracteres; cada palabra >= 3; máx. 80
        public static bool IsValidDescription(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsLettersSpacesAndDotsOnly(n)) return false;
            if (!TextRules.IsWordsMin3SingleSpacesAllowTrailingDot(n)) return false;
            return TextRules.MaxLen(n, 80);
        }

        // Precio: obligatorio y mayor que 0
        public static bool IsValidPrice(decimal? price) => price.HasValue && price.Value > 0m;

        // Stock: entero >= 0 (puede ser 0)
        public static bool IsValidStock(int? stock) => stock.HasValue && stock.Value >= 0;

        /// <summary>
        /// Normaliza campos para persistencia (Título y espacios).
        /// Llamar antes de guardar: ProductValidation.Normalize(p);
        /// </summary>
        public static void Normalize(Product p)
        {
            // Nombre en formato Título (cada palabra inicia con mayúscula)
            p.Name = TextRules.CanonicalTitle(p.Name);
            // Descripción: oración (primera letra mayúscula, resto minúscula) y colapsar espacios
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
    }
}
