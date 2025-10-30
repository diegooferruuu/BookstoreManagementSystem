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

        [BindProperty, Required(ErrorMessage = "La contrase�a actual es obligatoria."), Display(Name = "Contrase�a actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "La nueva contrase�a es obligatoria."), MinLength(8, ErrorMessage = "Debe tener al menos 8 caracteres."), Display(Name = "Nueva contrase�a")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Debe confirmar la nueva contrase�a."), Compare(nameof(NewPassword), ErrorMessage = "Las contrase�as no coinciden."), Display(Name = "Confirmar nueva contrase�a")]
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
                ModelState.AddModelError(nameof(CurrentPassword), "La contrase�a actual no es correcta.");
                return Page();
            }

            var regex = new Regex(@"^(?=.*[a-z�������])(?=.*[A-Z�������])(?=.*\d)(?=.*[\p{P}\p{S}]).{8,64}$", RegexOptions.CultureInvariant);
            if (!regex.IsMatch(NewPassword))
            {
                ModelState.AddModelError(nameof(NewPassword), "La nueva contrase�a debe incluir may�sculas, min�sculas, n�meros y al menos un car�cter especial.");
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
