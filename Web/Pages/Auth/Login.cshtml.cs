using System.Security.Claims;
using BookstoreManagementSystem.Application.DTOs;
using BookstoreManagementSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IJwtAuthService _authService;
        public LoginModel(IJwtAuthService authService) { _authService = authService; }

        [BindProperty]
        public AuthRequestDto Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Nunca prellenar el password
            Input.Password = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Extra server-side validation
            Input.UserOrEmail = (Input.UserOrEmail ?? string.Empty).Trim();
            Input.Password = (Input.Password ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(Input.UserOrEmail))
                ModelState.AddModelError("Input.UserOrEmail", "Ingrese su usuario o email.");
            if (string.IsNullOrWhiteSpace(Input.Password) || Input.Password.Length < 8)
                ModelState.AddModelError("Input.Password", "La contraseña debe tener al menos 8 caracteres.");

            if (!ModelState.IsValid)
            {
                // No re-mostrar la contraseña en el input
                ModelState.Remove("Input.Password");
                Input.Password = string.Empty;
                return Page();
            }

            if (!string.IsNullOrEmpty(ReturnUrl) && !Url.IsLocalUrl(ReturnUrl))
            {
                ReturnUrl = Url.Page("/Index");
            }

            var result = await _authService.SignInAsync(Input, HttpContext.RequestAborted);
            if (!result.Success || result.Value is null)
            {
                ErrorMessage = string.IsNullOrWhiteSpace(result.Error) ? "Usuario o contraseña inválidos." : result.Error;
                ModelState.Remove("Input.Password");
                Input.Password = string.Empty;
                return Page();
            }

            // Crear cookie de autenticación para Razor Pages
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Value.UserName),
                new(ClaimTypes.Email, result.Value.Email ?? string.Empty),
                new("given_name", result.Value.FirstName ?? string.Empty),
                new("family_name", result.Value.LastName ?? string.Empty),
                new("middle_name", result.Value.MiddleName ?? string.Empty)
            };
            foreach (var r in result.Value.Roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = result.Value.ExpiresAt.UtcDateTime
            });

            return Redirect(ReturnUrl ?? Url.Page("/Index")!);
        }
    }
}
