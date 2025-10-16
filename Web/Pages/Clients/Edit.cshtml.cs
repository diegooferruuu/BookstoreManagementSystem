using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ClientService _service;

        [BindProperty]
        public Client Client { get; set; } = new();

        [TempData]
        public Guid EditClientId { get; set; }

        public EditModel()
        {
            _service = new ClientService(new ClientRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditClientId"))
                return RedirectToPage("Index");

            Guid id = (Guid)TempData["EditClientId"];
            Client = _service.Read(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Client.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            _service.Update(Client);
            return RedirectToPage("Index");
        }
    }
}