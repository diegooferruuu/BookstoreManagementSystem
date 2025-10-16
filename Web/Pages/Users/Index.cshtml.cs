using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserService _service;

        public List<User> Users { get; set; } = new();
        public Dictionary<Guid, List<string>> UserRoles { get; set; } = new();

        public IndexModel()
        {
            _service = new UserService(new UserRepository());
        }

        public void OnGet()
        {
            Users = _service.GetAll();
            
            // Get roles for each user
            foreach (var user in Users)
            {
                UserRoles[user.Id] = _service.GetUserRoles(user.Id);
            }
        }

        public IActionResult OnPostDelete(Guid id)
        {
            _service.Delete(id);
            return RedirectToPage();
        }
    }
}
