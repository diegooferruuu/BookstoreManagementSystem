using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly ClientService _service;

        [BindProperty]
        public Client Client { get; set; } = new();

        public DeleteModel()
        {
            _service = new ClientService(new ClientRepository());
        }

        public IActionResult OnGet(int id)
        {
            Client = _service.Read(id);

            if (Client == null)
                return RedirectToPage("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            _service.Delete(Client.Id);
            return RedirectToPage("Index");
        }
    }
}