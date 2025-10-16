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

            var obj = TempData["EditClientId"];
            if (obj == null)
                return RedirectToPage("Index");

            Guid id;
            if (obj is Guid g)
                id = g;
            else if (obj is string s && Guid.TryParse(s, out g))
                id = g;
            else
                return RedirectToPage("Index");

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