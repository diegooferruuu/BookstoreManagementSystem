using ServiceUsers.Domain.Models;
using ServiceUsers.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _service;

        public List<User> Users { get; set; } = new();
        public Dictionary<Guid, List<string>> UserRoles { get; set; } = new();

        public IndexModel(IUserService service)
        {
            _service = service;
        }

        public void OnGet()
        {
            Users = _service.GetAll();
            
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
