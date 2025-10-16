using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly ClientService _service;

        public List<Client> Clients { get; set; } = new();

        public IndexModel()
        {
            _service = new ClientService(new ClientRepository());
        }

        public void OnGet()
        {
            Clients = _service.GetAll();
        }

        public IActionResult OnPostEdit(Guid id)
        {
            TempData["EditClientId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(Guid id)
        {
            _service.Delete(id);
            return RedirectToPage();
        }
    }
}