using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly ClientService _service;

        [BindProperty]
        public Client Client { get; set; } = new();

        public CreateModel()
        {
            _service = new ClientService(new ClientRepository());
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Client.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            _service.Create(Client);
            return RedirectToPage("Index");
        }
    }
}