using Microsoft.AspNetCore.Authentication;  // agrega esto
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Domain.Models;

namespace LibraryWeb.Pages.Users
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly PasswordHasher<User> _hasher;

        public ChangePasswordModel(IUserService userService)
        {
            _userService = userService;
            _hasher = new PasswordHasher<User>();
        }

        [BindProperty, Required(ErrorMessage = "La contraseña actual es obligatoria."), Display(Name = "Contraseña actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "La nueva contraseña es obligatoria."), MinLength(8, ErrorMessage = "Debe tener al menos 8 caracteres."), Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Debe confirmar la nueva contraseña."), Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden."), Display(Name = "Confirmar nueva contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "No se encontró el usuario actual.");
                return Page();
            }

            var user = _userService.GetAll().FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                return Page();
            }

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, CurrentPassword);
            if (verify == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(nameof(CurrentPassword), "La contraseña actual no es correcta.");
                return Page();
            }

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$");
            if (!regex.IsMatch(NewPassword))
            {
                ModelState.AddModelError(nameof(NewPassword),
                    "La nueva contraseña debe incluir mayúsculas, minúsculas, números y un carácter especial.");
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(nameof(ConfirmPassword), "Las contraseñas no coinciden.");
                return Page();
            }

            // Actualizar hash de contraseña
            user.PasswordHash = _hasher.HashPassword(user, NewPassword);
            _userService.Update(user);

            // Guardar mensaje de éxito
            TempData["SuccessMessage"] = "ok";

            // Cerrar sesión actual
            await HttpContext.SignOutAsync();

            return Page();
        }
    }
}
