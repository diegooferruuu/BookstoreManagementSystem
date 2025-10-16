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
        public int EditClientId { get; set; }

        public EditModel()
        {
            _service = new ClientService(new ClientRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditClientId"))
                return RedirectToPage("Index");

            var obj = TempData["EditClientId"];
            if (obj == null)
                return RedirectToPage("Index");

            int id = (int)obj;
            var client = _service.Read(id);
            if (client == null)
                return RedirectToPage("Index");

            Client = client;
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Client.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _service.Update(Client);
                return RedirectToPage("Index");
            }
            catch (ValidationException vex)
            {
                foreach (var e in vex.Errors)
                    ModelState.AddModelError($"Client.{e.Field}", e.Message);
                return Page();
            }
        }
    }
}