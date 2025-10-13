using System.Collections.Generic;
using BookstoreManagementSystem.Models;

namespace BookstoreManagementSystem.Validations
{
    public static class DistributorValidation
    {
        // Nombre: solo letras minúsculas, sin espacios, sin números, max 20
        public static bool IsValidName(string? s) =>
            TextRules.IsLowercaseLettersNoSpaces(s) &&
            TextRules.MinLen(s, 1) &&
            TextRules.MaxLen(s, 20);

        // Email: obligatorio si provisto, sin espacios, debe contener @ y terminar en .com
        public static bool IsValidEmail(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : TextRules.IsValidEmailNoSpacesAndCom(s);

        // Telefono: solo dígitos, sin espacios, max 15
        public static bool IsValidPhone(string? s) =>
            string.IsNullOrWhiteSpace(s) ? false : (TextRules.IsDigitsOnly(s) && TextRules.MaxLen(s, 15));

        // Dirección: puede tener caracteres especiales, máximo 50
        public static bool IsValidAddress(string? s) => string.IsNullOrWhiteSpace(s) || TextRules.MaxLen(s, 50);

        public static IEnumerable<ValidationError> Validate(Distributor d)
        {
            if (!IsValidName(d.Name))
                yield return new ValidationError(nameof(d.Name), "Nombre invalido (solo letras minúsculas, sin espacios, max 20).");

            if (!string.IsNullOrWhiteSpace(d.ContactEmail) && !IsValidEmail(d.ContactEmail))
                yield return new ValidationError(nameof(d.ContactEmail), "Email de contacto invalido (sin espacios, debe contener @ y terminar en .com).");

            if (!string.IsNullOrWhiteSpace(d.Phone) && !IsValidPhone(d.Phone))
                yield return new ValidationError(nameof(d.Phone), "Telefono invalido (solo numeros, sin espacios, max 15).");

            if (!IsValidAddress(d.Address))
                yield return new ValidationError(nameof(d.Address), "Direccion demasiado larga (max. 50).");
        }
    }
}