using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ServiceUsers.Application.DTOs;
using ServiceUsers.Application.Facade;

namespace LibraryWeb.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IUserFacade _userFacade;

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string SelectedRole { get; set; } = string.Empty;

        public CreateModel(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Email = (Email ?? string.Empty).Trim().ToLowerInvariant();
            SelectedRole = (SelectedRole ?? string.Empty).Trim();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var dto = new UserCreateDto
                {
                    Email = Email,
                    Role = SelectedRole
                };

                await _userFacade.CreateUserAsync(dto, HttpContext.RequestAborted);

                TempData["SuccessMessage"] = $"Usuario creado exitosamente. Las credenciales han sido enviadas a {Email}";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear el usuario: {ex.Message}");
                return Page();
            }
        }
    }
}
