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

        // Telefono: digitos y simbolos comunes
        private static readonly Regex RxPhone =
            new Regex(@"^[0-9\+\-\(\) ]+$", RegexOptions.Compiled);

        // Palabras (mayúsc./minúsc.) separadas por un único espacio; cada palabra con >= 2 letras
        private static readonly Regex RxWordsMin2SingleSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü]{2,}(?: [A-Za-zÁÉÍÓÚÑáéíóúÜü]{2,})*$",
                RegexOptions.Compiled);

        private static readonly Regex RxCollapseSpaces = new Regex(@"\s+", RegexOptions.Compiled);

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
    }
}
