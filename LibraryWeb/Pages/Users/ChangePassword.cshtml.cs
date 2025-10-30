using Microsoft.AspNetCore.Authentication;
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

        public IActionResult OnGet()
        {
            var username = User.Identity?.Name ?? TempData["PendingUser"]?.ToString();

            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Auth/Login");

            if (TempData.ContainsKey("PendingUser"))
                TempData.Keep("PendingUser");

            if (TempData.ContainsKey("FirstLogin"))
                ViewData["FirstLogin"] = true;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            string? username = User.Identity?.Name ?? TempData["PendingUser"]?.ToString();
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Auth/Login");

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

            var regex = new Regex(@"^(?=.*[a-záéíóúñç])(?=.*[A-ZÁÉÍÓÚÑÇ])(?=.*\d)(?=.*[\p{P}\p{S}]).{8,64}$", RegexOptions.CultureInvariant);
            if (!regex.IsMatch(NewPassword))
            {
                ModelState.AddModelError(nameof(NewPassword), "La nueva contraseña debe incluir mayúsculas, minúsculas, números y al menos un carácter especial.");
                return Page();
            }

            user.PasswordHash = _hasher.HashPassword(user, NewPassword);
            user.MustChangePassword = false;
            _userService.Update(user);

            await HttpContext.SignOutAsync();

            return RedirectToPage("/Auth/Login");
        }
    }
}
