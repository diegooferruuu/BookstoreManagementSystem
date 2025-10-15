using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        [TempData]
        public int EditDistributorId { get; set; }


        public EditModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet()
        {
            if (EditDistributorId == 0)
                return RedirectToPage("Index");

            Distributor = _repository.Read(EditDistributorId);

            if (Distributor == null)
                return RedirectToPage("Index");

            return Page();

        }

        public IActionResult OnPost()
        {
            foreach (var err in DistributorValidation.Validate(Distributor))
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

            _repository.Update(Distributor);
            return RedirectToPage("Index");
        }
    }
}