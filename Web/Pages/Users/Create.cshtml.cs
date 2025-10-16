using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Infrastructure.Factories;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BookstoreManagementSystem.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BookstoreManagementSystem.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IUsernameGenerator _usernameGenerator;
        private readonly IUserService _userService;
        private readonly PasswordHasher<User> _passwordHasher;

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string SelectedRole { get; set; } = string.Empty;

        public CreateModel(
            IEmailService emailService,
            IPasswordGenerator passwordGenerator,
            IUsernameGenerator usernameGenerator,
            IUserService userService)
        {
            _emailService = emailService;
            _passwordGenerator = passwordGenerator;
            _usernameGenerator = usernameGenerator;
            _userService = userService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult OnPost()
        {
            // Normalización previa
            Email = (Email ?? string.Empty).Trim().ToLowerInvariant();
            SelectedRole = (SelectedRole ?? string.Empty).Trim();

            // Validaciones adicionales del servidor
            var allowedRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Admin", "Employee" };
            if (!allowedRoles.Contains(SelectedRole))
                ModelState.AddModelError("SelectedRole", "Debe seleccionar un rol válido.");

            // Duplicados de correo
            var emailExists = _userService.GetAll()
                .Any(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
                ModelState.AddModelError("Email", "El correo ya está registrado.");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                
                // 1. Generar username único desde el email
                var baseUsername = _usernameGenerator.GenerateUsernameFromEmail(Email);
                var uniqueUsername = _usernameGenerator.EnsureUniqueUsername(
                    baseUsername,
                    username => _userService.GetAll().Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                );

                // Duplicados de usuario por si el generador no cubre un caso extremo
                var usernameExists = _userService.GetAll().Any(u => u.Username.Equals(uniqueUsername, StringComparison.OrdinalIgnoreCase));
                if (usernameExists)
                {
                    ModelState.AddModelError("Email", "No fue posible generar un usuario único. Intente con otro correo.");
                    return Page();
                }

                // 2. Generar contraseña segura
                var generatedPassword = _passwordGenerator.GenerateSecurePassword();

                // 3. Hashear la contraseña para guardarla en la BD
                var tempUser = new User(); // temporal para el hasher
                var passwordHash = _passwordHasher.HashPassword(tempUser, generatedPassword);

                // 4. Crear el usuario
                var user = new User
                {
                    Email = Email,
                    Username = uniqueUsername,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    PasswordHash = passwordHash,
                    IsActive = true
                };

                _userService.Create(user);

                // 5. Asignar rol
                var createdUser = _userService.GetAll()
                    .FirstOrDefault(u => u.Username.Equals(uniqueUsername, StringComparison.OrdinalIgnoreCase));
                
                if (createdUser != null)
                {
                    _userService.UpdateUserRoles(createdUser.Id, new List<string> { SelectedRole });
                }

                // 6. Enviar email con la contraseña en texto plano (antes de hashear)
                var emailSubject = "Bienvenido - Tu cuenta ha sido creada";
                var emailBody = GenerateWelcomeEmailHtml(uniqueUsername, generatedPassword);
                
                _ = _emailService.SendEmailAsync(Email, emailSubject, emailBody);
                // Note: Fire and forget - en producción considera manejar fallos

                TempData["SuccessMessage"] = $"Usuario creado exitosamente. Las credenciales han sido enviadas a {Email}";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear el usuario: {ex.Message}");
                return Page();
            }
        }

        private string GenerateWelcomeEmailHtml(string username, string password)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                        .credentials {{ background-color: white; padding: 20px; border-left: 4px solid #007bff; margin: 20px 0; }}
                        .credential-item {{ margin: 10px 0; }}
                        .credential-label {{ font-weight: bold; color: #007bff; }}
                        .credential-value {{ font-family: 'Courier New', monospace; font-size: 16px; color: #333; }}
                        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
                        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Bookstore Management System</h1>
                        </div>
                        <div class='content'>
                            <h2>¡Bienvenido!</h2>
                            <p>Tu cuenta ha sido creada exitosamente. A continuación encontrarás tus credenciales de acceso:</p>
                            
                            <div class='credentials'>
                                <div class='credential-item'>
                                    <span class='credential-label'>Usuario:</span><br/>
                                    <span class='credential-value'>{username}</span>
                                </div>
                                <div class='credential-item'>
                                    <span class='credential-label'>Contraseña:</span><br/>
                                    <span class='credential-value'>{password}</span>
                                </div>
                            </div>
                            
                            <div class='warning'>
                                <strong>⚠️ Importante:</strong> Por favor, guarda estas credenciales en un lugar seguro.
                            </div>
                            
                            <p>Puedes acceder al sistema utilizando estas credenciales.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                            <p>&copy; 2025 Bookstore Management System. Todos los derechos reservados.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
