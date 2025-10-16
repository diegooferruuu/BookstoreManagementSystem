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
            // Ejecutar validaciones del Domain
            foreach (var err in DistributorValidation.Validate(Distributor))
                ModelState.AddModelError($"Distributor.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            // Normalizar
            DistributorValidation.Normalize(Distributor);

            _service.Create(Distributor);
            return RedirectToPage("Index");
        }
    }
}