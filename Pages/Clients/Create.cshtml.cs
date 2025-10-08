using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        [BindProperty]
        public Client Client { get; set; } = new();

        public CreateModel()
        {
            _repository = new ClientRepository();
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