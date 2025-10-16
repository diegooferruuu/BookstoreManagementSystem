using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq;

namespace BookstoreManagementSystem.Domain.Validations
{
    internal static class TextRules
    {
        // Solo letras (incluye tildes/ñ) y espacios/apóstrofo
        private static readonly Regex RxLettersSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü' ]+$", RegexOptions.Compiled);

        // Solo letras (incluye tildes/ñ) sin espacios
        private static readonly Regex RxLettersOnly =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü]+$", RegexOptions.Compiled);

        // Telefono: digitos y simbolos comunes
        private static readonly Regex RxPhone =
            new Regex(@"^[0-9\+\-\(\) ]+$", RegexOptions.Compiled);

        // Palabras (mayúsc./minúsc.) separadas por un único espacio; cada palabra con >= 2 letras
        private static readonly Regex RxWordsMin2SingleSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü]{2,}(?: [A-Za-zÁÉÍÓÚÑáéíóúÜü]{2,})*$",
                RegexOptions.Compiled);

        private static readonly Regex RxCollapseSpaces = new Regex(@"\s+", RegexOptions.Compiled);

        // Letras (con acentos), espacios y puntos
        private static readonly Regex RxLettersSpacesDotsOnly =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü. ]+$", RegexOptions.Compiled);

        public static string Normalize(string? s) => (s ?? string.Empty).Trim();

        public static string NormalizeSpaces(string? s)
        {
            var n = Normalize(s);
            if (n.Length == 0) return n;
            return RxCollapseSpaces.Replace(n, " ");
        }

        /// <summary>
        /// Convierte a "Título": primera letra mayúscula y el resto minúsculas por palabra.
        /// También recorta y colapsa espacios.
        /// </summary>
        public static string CanonicalTitle(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s).ToLower(culture);
            // TextInfo.ToTitleCase maneja acentos correctamente; ya forzamos a lower antes.
            n = culture.TextInfo.ToTitleCase(n);
            return n;
        }

        /// <summary>
        /// Convierte a "Sentence case": primera letra en mayúscula y el resto en minúscula.
        /// Respeta puntos y colapsa espacios.
        /// </summary>
        public static string CanonicalSentence(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s).ToLower(culture);
            if (string.IsNullOrEmpty(n)) return n;
            // Primera letra alfabética en mayúscula
            var chars = n.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i], culture);
                    break;
                }
            }
            // Quitar espacios antes de punto: " ." -> "."
            var result = new string(chars);
            result = Regex.Replace(result, @"\s+\.", ".");
            return result;
        }

        /// <summary>
        /// Solo letras (con acentos) y espacios únicos, sin palabras de 1 letra.
        /// </summary>
        public static bool IsWordsLettersWithSingleSpacesMin2(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxWordsMin2SingleSpaces.IsMatch(NormalizeSpaces(s));

        /// <summary>
        /// Rechaza cadenas con letras separadas por espacios ("c o c a"), es decir, tokens de 1 letra.
        /// </summary>
        public static bool HasNoSpacedOutLetters(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            var parts = n.Split(' ');
            return parts.All(p => p.Length >= 2);
        }

        public static bool IsLettersAndSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpaces.IsMatch(NormalizeSpaces(s));

        /// <summary>
        /// Solo letras, espacios y puntos. Sin caracteres especiales distintos a '.'
        /// </summary>
        public static bool IsLettersSpacesAndDotsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpacesDotsOnly.IsMatch(NormalizeSpaces(s));

        public static bool NotAllUppercase(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            var letters = new string(n.Where(char.IsLetter).ToArray());
            if (letters.Length == 0) return true; // nada que evaluar
            return letters != letters.ToUpper();
        }

        public static bool HasNoDigits(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : !NormalizeSpaces(s).Any(char.IsDigit);

        public static bool IsValidEmail(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            try { _ = new MailAddress(n); return true; }
            catch { return false; }
        }

        // email obligado: no espacios, contiene '@' y termina en ".com"
        public static bool IsValidEmailNoSpacesAndCom(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (n.Contains(' ')) return false;
            if (!n.Contains('@')) return false;
            if (!n.EndsWith(".com", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        public static bool IsValidPhone(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!RxPhone.IsMatch(n)) return false;
            var digits = n.Count(char.IsDigit);
            return digits >= 7 && digits <= 20;
        }

        // Solo digitos, sin espacios
        public static bool IsDigitsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(Normalize(s), "^\\d+$");

        public static bool IsNonNegative(decimal value) => value >= 0m;
        public static bool IsNonNegativeInt(int value) => value >= 0;
        public static bool MinLen(string? s, int len) => NormalizeSpaces(s).Length >= len;
        public static bool MaxLen(string? s, int len) => NormalizeSpaces(s).Length <= len;

        public static bool LenEquals(string? s, int len) => Normalize(s).Length == len;

        public static bool IsSingleWordLettersOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersOnly.IsMatch(Normalize(s));

        /// <summary>
        /// Valida palabras separadas por un solo espacio, cada palabra con >= 2 letras.
        /// Permite un punto '.' al final de las palabras (no dentro) y no otros caracteres.
        /// </summary>
        public static bool IsWordsMin2SingleSpacesAllowTrailingDot(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!IsLettersSpacesAndDotsOnly(n)) return false;

            var parts = n.Split(' ');
            foreach (var raw in parts)
            {
                var word = raw;
                if (word.EndsWith('.')) word = word.Substring(0, word.Length - 1);
                if (word.Length < 2) return false;
                // Debe contener solo letras
                if (!word.All(char.IsLetter)) return false;
            }
            return true;
        }

        /// <summary>
        /// Igual que la anterior pero exige >= 3 letras por palabra.
        /// </summary>
        public static bool IsWordsMin3SingleSpacesAllowTrailingDot(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!IsLettersSpacesAndDotsOnly(n)) return false;

            var parts = n.Split(' ');
            foreach (var raw in parts)
            {
                var word = raw;
                if (word.EndsWith('.')) word = word[..^1];
                if (word.Length < 3) return false;
                if (!word.All(char.IsLetter)) return false;
            }
            return true;
        }
    }
}
