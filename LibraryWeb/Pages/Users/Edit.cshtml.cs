using ServiceUsers.Domain.Models;
using ServiceUsers.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _service;

        [BindProperty]
        public Guid UserId { get; set; }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedRole { get; set; } = string.Empty;

        [TempData]
        public Guid EditUserId { get; set; }

        public EditModel(IUserService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            if (EditUserId == Guid.Empty)
                return RedirectToPage("Index");

            var user = _service.Read(EditUserId);

            if (user == null)
                return RedirectToPage("Index");

            UserId = user.Id;
            Email = user.Email;

            var roles = _service.GetUserRoles(user.Id);
            SelectedRole = roles.FirstOrDefault() ?? string.Empty;

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico es requerido");
            }

            if (string.IsNullOrWhiteSpace(SelectedRole))
            {
                ModelState.AddModelError("SelectedRole", "El rol es requerido");
            }

            if (!ModelState.IsValid)
                return Page();

            var user = _service.Read(UserId);
            if (user == null)
                return RedirectToPage("Index");

            user.Email = Email.Trim().ToLowerInvariant();
            user.Username = Email.Split('@')[0].ToLowerInvariant();
            
            _service.Update(user);

            if (!string.IsNullOrWhiteSpace(SelectedRole))
            {
                _service.UpdateUserRoles(UserId, new List<string> { SelectedRole });
            }

            return RedirectToPage("Index");
        }
    }
}
