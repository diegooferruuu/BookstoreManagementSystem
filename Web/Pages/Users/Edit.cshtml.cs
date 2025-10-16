using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserService _service;

        [BindProperty]
        public int UserId { get; set; }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedRole { get; set; } = string.Empty;

        [TempData]
        public int EditUserId { get; set; }

        public EditModel()
        {
            _service = new UserService(new UserRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditUserId"))
                return RedirectToPage("Index");

            int id = (int)TempData["EditUserId"];
            var user = _service.Read(id);
            
            if (user == null)
                return RedirectToPage("Index");

            UserId = user.Id;
            Email = user.Email;

            // Get current role
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

            // Update user
            user.Email = Email.Trim().ToLowerInvariant();
            user.Username = Email.Split('@')[0].ToLowerInvariant();
            
            _service.Update(user);

            // Update role
            if (!string.IsNullOrWhiteSpace(SelectedRole))
            {
                _service.UpdateUserRoles(UserId, new List<string> { SelectedRole });
            }

            return RedirectToPage("Index");
        }
    }
}
