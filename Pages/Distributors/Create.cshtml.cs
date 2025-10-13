using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using BookstoreManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class CreateModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public CreateModel()
        {
            _repository = new DistributorRepository();
        }

        public IActionResult OnPost()
        {
            // Ejecutar validaciones personalizadas y añadir errores a ModelState
            foreach (var err in Validations.DistributorValidation.Validate(Distributor))
                ModelState.AddModelError($"Distributor.{err.Field}", err.Message);

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

            _repository.Create(Distributor);
            return RedirectToPage("Index");
        }
    }
}