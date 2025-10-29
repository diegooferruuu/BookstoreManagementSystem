using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ServiceClients.Domain.Validations
{
    internal static class TextRules
    {
        private static readonly Regex RxLettersSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ' ]+$", RegexOptions.Compiled);

        private static readonly Regex RxLettersOnly =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ]+$", RegexOptions.Compiled);

        private static readonly Regex RxAddressAllowed =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9 .\/]+$", RegexOptions.Compiled);

        private static readonly Regex RxCollapseSpaces = new Regex(@"\s+", RegexOptions.Compiled);

        private static readonly HashSet<string> SpanishParticles = new(StringComparer.OrdinalIgnoreCase)
        {
            "de", "del", "la", "las", "los", "y", "en", "para", "san", "santa"
        };

        public static string Normalize(string? s) => (s ?? string.Empty).Trim();

        public static string NormalizeSpaces(string? s)
        {
            var n = Normalize(s);
            if (n.Length == 0) return n;
            return RxCollapseSpaces.Replace(n, " ");
        }

        public static string CanonicalTitle(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s).ToLower(culture);
            n = culture.TextInfo.ToTitleCase(n);
            return n;
        }

        public static bool IsLettersAndSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpaces.IsMatch(NormalizeSpaces(s));

        public static bool IsDigitsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(Normalize(s), "^\\d+$");

        public static bool MinLen(string? s, int len) => NormalizeSpaces(s).Length >= len;
        public static bool MaxLen(string? s, int len) => NormalizeSpaces(s).Length <= len;
        public static bool LenEquals(string? s, int len) => Normalize(s).Length == len;

        public static bool IsSingleWordLettersOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersOnly.IsMatch(Normalize(s));

        public static bool IsCompoundLastName(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!IsLettersAndSpaces(n)) return false;
            var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return false;
            var hasMain = false;
            foreach (var part in parts)
            {
                if (!part.All(char.IsLetter)) return false;
                if (SpanishParticles.Contains(part)) continue;
                if (part.Length < 3) return false;
                hasMain = true;
            }
            return hasMain;
        }

        public static bool IsValidCbbaAddress(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!RxAddressAllowed.IsMatch(n)) return false;
            var tokens = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                var raw = tokens[i];
                if (string.Equals(raw, "S/N", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var t = raw;
                var endsWithDot = t.EndsWith('.');
                if (endsWithDot) t = t[..^1];
                var upper = t.ToUpperInvariant();
                if (upper.StartsWith("N") || upper.StartsWith("NO") || upper.StartsWith("NRO"))
                {
                    var rest = raw.Substring(upper.StartsWith("NRO") ? (endsWithDot ? 4 : 3) : (upper.StartsWith("NO") ? (endsWithDot ? 3 : 2) : (endsWithDot ? 2 : 1)));
                    if (rest.Length > 0)
                    {
                        if (!rest.All(char.IsDigit)) return false;
                        continue;
                    }
                    if (i + 1 < tokens.Length && tokens[i + 1].All(char.IsDigit)) { i++; continue; }
                    return false;
                }
                var lettersOnly = t.All(char.IsLetter);
                if (lettersOnly)
                {
                    if (t.Length < 2) return false;
                    continue;
                }
                if (t.All(char.IsDigit))
                {
                    continue;
                }
                return false;
            }
            return true;
        }
    }
}
