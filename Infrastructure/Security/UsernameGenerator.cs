using BookstoreManagementSystem.Domain.Interfaces;

namespace BookstoreManagementSystem.Infrastructure.Security
{
    public class UsernameGenerator : IUsernameGenerator
    {
        public string GenerateUsernameFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            // Extraer la parte local del email (antes del @)
            var localPart = email.Split('@')[0];
            
            // Limpiar caracteres especiales y convertir a minúsculas
            return localPart.ToLowerInvariant().Trim();
        }

        public string EnsureUniqueUsername(string baseUsername, Func<string, bool> existsCheck)
        {
            if (string.IsNullOrWhiteSpace(baseUsername))
                throw new ArgumentException("Base username cannot be null or empty", nameof(baseUsername));

            var username = baseUsername;
            var suffix = 1;

            // Si el username ya existe, agregar un sufijo numérico
            while (existsCheck(username))
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
                
                // Prevenir loops infinitos
                if (suffix > 9999)
                    throw new InvalidOperationException("Could not generate unique username");
            }

            return username;
        }
    }
}
