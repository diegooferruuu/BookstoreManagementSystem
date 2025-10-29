using System.Security.Claims;
using ServiceUsers.Application.DTOs;
using ServiceUsers.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Auth
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
            Input.Password = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Input.UserOrEmail = (Input.UserOrEmail ?? string.Empty).Trim();
            Input.Password = (Input.Password ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(Input.UserOrEmail))
                ModelState.AddModelError("Input.UserOrEmail", "El campo Usuario o Correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(Input.Password))
                ModelState.AddModelError("Input.Password", "El campo Contraseña es obligatorio.");

            if (!ModelState.IsValid)
                return Page();

            if (!string.IsNullOrEmpty(ReturnUrl) && !Url.IsLocalUrl(ReturnUrl))
                ReturnUrl = Url.Page("/Index");

            var result = await _authService.SignInAsync(Input, HttpContext.RequestAborted);
            if (!result.IsSuccess || result.Value is null)
            {
                ErrorMessage = "Credenciales inválidas.";
                return Page();
            }

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

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = result.Value.ExpiresAt.UtcDateTime
                });

            return Redirect(ReturnUrl ?? Url.Page("/Index")!);
        }
    }
}
