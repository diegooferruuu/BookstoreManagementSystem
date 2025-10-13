using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Validations;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        [BindProperty]
        public Client Client { get; set; } = new();

        public EditModel()
        {
            var creator = new ClientCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet(int id)
        {
            var Client = _repository.Read(id);
            
            if (Client == null)
                return RedirectToPage("Index");

            this.Client = Client;
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Cliente.{err.Field}", err.Message);

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

            _repository.Create(Client);
            return RedirectToPage("Index");
        }
    }
}