using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ServiceDistributors.Domain.Models;
using ServiceCommon.Domain.Validations;

namespace ServiceDistributors.Domain.Validations
{
    public static class DistributorValidation
    {
        private static readonly Regex RxNameAllowed =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9&.' \-]+$", RegexOptions.Compiled);

        private static readonly HashSet<string> Connectors = new(System.StringComparer.OrdinalIgnoreCase)
        {
            "de","del","la","las","los","el","y","en","para","por","con","san","santa"
        };

        private static readonly HashSet<string> LegalSuffixes = new(System.StringComparer.OrdinalIgnoreCase)
        {
            "SRL","SA","LTDA","EIRL","CIA","CÍA"
        };

        public static bool IsValidName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var n = TextRules.NormalizeSpaces(s);
            if (!RxNameAllowed.IsMatch(n)) return false;
            if (!TextRules.MaxLen(n, 60)) return false;

            var tokens = n.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var hasMainWord = false;
            for (int i = 0; i < tokens.Length; i++)
            {
                var raw = tokens[i];
                if (Connectors.Contains(raw)) continue;

                var noDots = raw.Replace(".", string.Empty).ToUpperInvariant();
                if (LegalSuffixes.Contains(noDots)) continue;

                if (raw.All(char.IsLetter))
                {
                    if (raw.Length < 3) return false;
                    hasMainWord = true;
                    continue;
                }
                if (raw.All(char.IsDigit))
                {
                    continue;
                }

                string[] segs = raw.Split(new[] { '&', '-', '\'', }, System.StringSplitOptions.RemoveEmptyEntries);
                if (segs.Length == 0) return false;
                bool allSegsOk = true;
                int lettersInToken = 0;
                foreach (var seg in segs)
                {
                    if (string.IsNullOrWhiteSpace(seg)) { allSegsOk = false; break; }
                    var allValidChars = seg.All(ch => char.IsLetterOrDigit(ch));
                    if (!allValidChars) { allSegsOk = false; break; }
                    lettersInToken += seg.Count(char.IsLetter);
                }
                if (!allSegsOk) return false;
                if (lettersInToken >= 3) hasMainWord = true;
            }

            return hasMainWord;
        }

        public static bool IsValidEmail(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsValidEmail(s) && TextRules.MaxLen(s, 100);

        public static bool IsValidPhone(string? s) =>
            !string.IsNullOrWhiteSpace(s) && TextRules.IsDigitsOnly(s) && TextRules.LenEquals(s, 8);

        public static bool IsValidAddress(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return true;
            var n = TextRules.NormalizeSpaces(s);
            if (!TextRules.IsValidCbbaAddress(n)) return false;
            return TextRules.MaxLen(n, 60);
        }

        public static void Normalize(Distributor d)
        {
            d.Name = TextRules.CanonicalBusinessName(d.Name);
            if (!string.IsNullOrWhiteSpace(d.Address))
                d.Address = TextRules.CanonicalTitle(d.Address);
        }

        public static IEnumerable<ValidationError> Validate(Distributor d)
        {
            if (!IsValidName(d.Name))
                yield return new ValidationError(nameof(d.Name),
                    "Nombre inválido. Solo letras, dígitos, espacios y & . - '. Conectores 'de/del/la/…' permitidos. Debe incluir al menos una palabra principal (≥3 letras).");

            if (!IsValidEmail(d.ContactEmail))
                yield return new ValidationError(nameof(d.ContactEmail),
                    "Correo inválido.");

            if (!IsValidPhone(d.Phone))
                yield return new ValidationError(nameof(d.Phone),
                    "Teléfono inválido. Debe tener 8 dígitos.");

            if (!IsValidAddress(d.Address))
                yield return new ValidationError(nameof(d.Address),
                    "Dirección inválida. Formatos comunes: 'Av. América Este N1759', 'Calle Corrales S/N', 'No. 1234'. Solo letras, números, espacios, punto y '/'.");
        }

        public static ServiceCommon.Domain.Results.Result ValidateAsResult(Distributor d)
            => ServiceCommon.Domain.Results.Result.FromValidation(Validate(d));

        public static ServiceCommon.Domain.Results.Result<Distributor> ValidateAndWrap(Distributor d)
        {
            var errors = Validate(d).ToList();
            return errors.Count == 0
                ? ServiceCommon.Domain.Results.Result<Distributor>.Ok(d)
                : ServiceCommon.Domain.Results.Result<Distributor>.FromErrors(errors);
        }
    }
}
