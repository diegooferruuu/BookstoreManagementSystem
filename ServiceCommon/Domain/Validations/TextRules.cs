using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace ServiceCommon.Domain.Validations
{
    public static class TextRules
    {
        private static readonly Regex RxLettersSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ' ]+$", RegexOptions.Compiled);

        private static readonly Regex RxLettersOnly =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ]+$", RegexOptions.Compiled);

        private static readonly Regex RxLettersDigitsSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9. ]+$", RegexOptions.Compiled);

        private static readonly Regex RxLettersDigitsSpacesDotsCommas =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9 ,.]+$", RegexOptions.Compiled);

        private static readonly Regex RxPhone =
            new Regex(@"^[0-9\+\-\(\) ]+$", RegexOptions.Compiled);

        private static readonly Regex RxWordsMin2SingleSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ]{2,}(?: [A-Za-zÁÉÍÓÚÑáéíóúÜüñ]{2,})*$",
                RegexOptions.Compiled);

        private static readonly Regex RxCollapseSpaces = new Regex(@"\s+", RegexOptions.Compiled);

        private static readonly Regex RxLettersSpacesDotsOnly =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ. ]+$", RegexOptions.Compiled);

        private static readonly Regex RxAddressAllowed =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9 .\/]+$", RegexOptions.Compiled);

        private static readonly HashSet<string> SpanishParticles = new(StringComparer.OrdinalIgnoreCase)
        {
            "de", "del", "la", "las", "los", "y", "en", "para", "san", "santa"
        };

        private static readonly HashSet<string> AllowedConnectors = new(StringComparer.OrdinalIgnoreCase)
        {
            "de", "del", "para", "con", "y", "en", "por"
        };

        private static readonly HashSet<string> AllowedUnits = new(StringComparer.OrdinalIgnoreCase)
        {
            "gr", "g", "kg", "ml", "l", "cm", "mm", "m"
        };

        private static readonly Dictionary<string, string> UpperBusinessTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            ["srl"] = "SRL",
            ["sa"] = "S.A.",
            ["ltda"] = "LTDA",
            ["eirl"] = "EIRL"
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

        public static string CanonicalBusinessName(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return n;

            var titled = culture.TextInfo.ToTitleCase(n.ToLower(culture));
            var parts = titled.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                var token = parts[i];
                var key = token.Replace(".", string.Empty);
                if (UpperBusinessTokens.TryGetValue(key, out var canonical))
                {
                    parts[i] = canonical;
                }
            }
            return string.Join(' ', parts);
        }

        public static string CanonicalProductName(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s).ToLower(culture);
            if (string.IsNullOrWhiteSpace(n)) return n;
            var parts = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                var token = parts[i];
                var tokenNoDot = token.EndsWith('.') ? token[..^1] : token;
                if (SpanishParticles.Contains(token) || token.All(char.IsDigit) || AllowedUnits.Contains(tokenNoDot))
                {
                    parts[i] = token;
                }
                else if (tokenNoDot.All(char.IsLetter) && tokenNoDot.Length >= 4)
                {
                    parts[i] = char.ToUpper(tokenNoDot[0], culture) + tokenNoDot.Substring(1) + (token.EndsWith('.') ? "." : string.Empty);
                }
                else
                {
                    parts[i] = token;
                }
            }
            return string.Join(' ', parts);
        }

        public static string CanonicalSentence(string? s)
        {
            var culture = new CultureInfo("es-ES");
            var n = NormalizeSpaces(s).ToLower(culture);
            if (string.IsNullOrEmpty(n)) return n;

            var result = Regex.Replace(n, @"\s+\.", ".");
            result = Regex.Replace(result, @"\.(\S)", ". $1");

            var chars = result.ToCharArray();
            bool capitalizeNext = true;
            for (int i = 0; i < chars.Length; i++)
            {
                if (capitalizeNext && char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i], culture);
                    capitalizeNext = false;
                    continue;
                }

                if (chars[i] == '.' || chars[i] == '!' || chars[i] == '?')
                {
                    capitalizeNext = true;
                }
            }
            return new string(chars);
        }

        public static bool IsLettersDigitsAndSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersDigitsSpaces.IsMatch(NormalizeSpaces(s));

        public static bool IsLettersDigitsSpacesDotsCommasOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersDigitsSpacesDotsCommas.IsMatch(NormalizeSpaces(s));

        public static bool IsValidProductName(string? s)
        {
            var errors = GetProductNameErrors(s);
            return !errors.Any();
        }

        public static IEnumerable<string> GetProductNameErrors(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n))
            {
                yield return "El nombre es obligatorio.";
                yield break;
            }

            if (!IsLettersDigitsAndSpaces(n))
                yield return "Solo letras y números.";

            var tokens = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var hasMain = false;
            foreach (var t in tokens)
            {
                var token = t.Trim();
                if (string.IsNullOrEmpty(token)) continue;
                if (AllowedConnectors.Contains(token) || SpanishParticles.Contains(token))
                    continue;
                if (token.Contains('.') && !token.EndsWith('.'))
                {
                    yield return "Solo puntos y no otros caracteres especiales.";
                    continue;
                }
                var unit = token.TrimEnd('.');
                if (AllowedUnits.Contains(unit))
                    continue;
                if (unit.All(char.IsDigit))
                    continue;
                if (unit.All(char.IsLetter))
                {
                    if (unit.Length < 3)
                        yield return "Mínimo 3 letras por palabra.";
                    else
                        hasMain = true;
                    continue;
                }
                if (unit.Any(char.IsLetter) && unit.Any(char.IsDigit))
                {
                    hasMain = true;
                    continue;
                }
                yield return "Solo puntos y no otros caracteres especiales.";
            }
            if (!hasMain)
                yield return "Incluya al menos una palabra con 3 letras.";
        }

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
                if (!word.All(char.IsLetter)) return false;
            }
            return true;
        }

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

        public static bool IsValidProductDescription(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!IsLettersDigitsSpacesDotsCommasOnly(n)) return false;
            var parts = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in parts)
            {
                var token = raw.TrimEnd('.', ',') ;
                if (token.Length == 0) continue;
                if (SpanishParticles.Contains(token) || AllowedConnectors.Contains(token)) continue;
                if (token.All(char.IsDigit)) continue;
                if (token.All(char.IsLetter) && token.Length >= 3) continue;
                if (token.Any(char.IsLetter) && token.Any(char.IsDigit)) continue;
                return false;
            }
            return true;
        }

        public static bool IsValidProductDescriptionLoose(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!IsLettersDigitsSpacesDotsCommasOnly(n)) return false;
            var parts = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var hasWord = false;
            foreach (var raw in parts)
            {
                var token = raw.TrimEnd('.', ',');
                if (token.Length == 0) continue;
                if (SpanishParticles.Contains(token) || AllowedConnectors.Contains(token)) continue;
                if (token.All(char.IsDigit)) continue;
                if (token.All(char.IsLetter) && token.Length >= 2) { hasWord = true; continue; }
                if (token.Any(char.IsLetter) && token.Any(char.IsDigit)) { hasWord = true; continue; }
                return false;
            }
            return hasWord;
        }

        public static bool IsLettersAndSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpaces.IsMatch(NormalizeSpaces(s));
        public static bool IsLettersSpacesAndDotsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpacesDotsOnly.IsMatch(NormalizeSpaces(s));
        public static bool IsDigitsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(Normalize(s), "^\\d+$");
        public static bool IsNonNegative(decimal value) => value >= 0m;
        public static bool IsNonNegativeInt(int value) => value >= 0;
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
            var parts = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
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

        public static bool IsValidEmail(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            try { _ = new MailAddress(n); return true; }
            catch { return false; }
        }

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

        public static bool NotAllUppercase(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            var letters = new string(n.Where(char.IsLetter).ToArray());
            if (letters.Length == 0) return true;
            return letters != letters.ToUpper();
        }

        public static bool HasNoDigits(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : !NormalizeSpaces(s).Any(char.IsDigit);

        public static bool IsWordsLettersWithSingleSpacesMin2(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxWordsMin2SingleSpaces.IsMatch(NormalizeSpaces(s));

        public static bool IsValidCbbaAddress(string? s)
        {
            var n = NormalizeSpaces(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!RxAddressAllowed.IsMatch(n)) return false;
            var tokens = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                var raw = tokens[i];
                if (string.Equals(raw, "S/N", System.StringComparison.OrdinalIgnoreCase))
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
