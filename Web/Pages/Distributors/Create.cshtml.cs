using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class CreateModel : PageModel
    {
        private readonly DistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public CreateModel()
        {
            _service = new DistributorService(new DistributorRepository());
        }

        public IActionResult OnPost()
        {
            foreach (var err in DistributorValidation.Validate(Distributor))
                ModelState.AddModelError($"Distributor.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _service.Create(Distributor);
                return RedirectToPage("Index");
            }
            catch (ValidationException vex)
            {
                foreach (var e in vex.Errors)
                    ModelState.AddModelError($"Distributor.{e.Field}", e.Message);
                return Page();
            }
        }
    }
}