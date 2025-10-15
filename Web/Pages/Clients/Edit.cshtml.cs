using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        [BindProperty]
        public Client Client { get; set; } = new();

        [TempData]
        public int EditClientId { get; set; }

        public EditModel()
        {
            var creator = new ClientCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditClientId"))
                return RedirectToPage("Index");

            int id = (int)TempData["EditClientId"];
            Client = _repository.Read(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Client.{err.Field}", err.Message);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($" Campo: {error.Key} - Error: {subError.ErrorMessage}");
                    }
                }
                return Page();
            }

            _repository.Update(Client);
            return RedirectToPage("Index");
        }
    }
}