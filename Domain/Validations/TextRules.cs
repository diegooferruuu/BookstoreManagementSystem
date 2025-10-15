using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BookstoreManagementSystem.Domain.Validations
{
    internal static class TextRules
    {
        // Solo letras (incluye tildes/ñ) y espacios
        private static readonly Regex RxLettersSpaces =
            new Regex(@"^[A-Za-zÁÉÍÓÚÑáéíóúÜü' ]+$", RegexOptions.Compiled);

        // Solo letras minusculas (incluye tildes/ñ), sin espacios
        private static readonly Regex RxLowerLettersNoSpaces =
            new Regex(@"^[a-záéíóúñü]+$", RegexOptions.Compiled);

        // Telefono: digitos y simbolos comunes
        private static readonly Regex RxPhone =
            new Regex(@"^[0-9\+\-\(\) ]+$", RegexOptions.Compiled);

        public static string Normalize(string? s) => (s ?? string.Empty).Trim();

        public static bool IsLettersAndSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLettersSpaces.IsMatch(Normalize(s));

        // comprueba que la cadena solo tenga letras en minuscula y NO tenga espacios
        public static bool IsLowercaseLettersNoSpaces(string? s) =>
            !string.IsNullOrWhiteSpace(s) && RxLowerLettersNoSpaces.IsMatch(Normalize(s));
        // comprueba que la cadena solo tenga letras en minuscula
        public static bool IsLowercaseLetters(string? s) =>
            RxLettersSpaces.IsMatch(Normalize(s));

        public static bool NotAllUppercase(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false; // si es requerido que falle aquí
            var letters = new string(n.Where(char.IsLetter).ToArray());
            if (letters.Length == 0) return true; // nada que evaluar
            return letters != letters.ToUpper();
        }

        public static bool HasNoDigits(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : !Normalize(s).Any(char.IsDigit);

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
            // longitud aceptable se verifica en el llamador si es necesario
            return true;
        }

        public static bool IsValidPhone(string? s)
        {
            var n = Normalize(s);
            if (string.IsNullOrWhiteSpace(n)) return false;
            if (!RxPhone.IsMatch(n)) return false;
            // al menos 7 digitos reales
            var digits = n.Count(char.IsDigit);
            return digits >= 7 && digits <= 20;
        }

        // Solo digitos, sin espacios
        public static bool IsDigitsOnly(string? s) =>
            !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(Normalize(s), "^\\d+$");

        public static bool IsNonNegative(decimal value) => value >= 0m;
        public static bool IsNonNegativeInt(int value) => value >= 0;
        public static bool MinLen(string? s, int len) => Normalize(s).Length >= len;
        public static bool MaxLen(string? s, int len) => Normalize(s).Length <= len;
    }
}
