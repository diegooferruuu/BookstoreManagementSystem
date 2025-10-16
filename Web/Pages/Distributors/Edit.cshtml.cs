using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class EditModel : PageModel
    {
        private readonly DistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        [TempData]
        public int EditDistributorId { get; set; }

        public EditModel()
        {
            _service = new DistributorService(new DistributorRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditDistributorId"))
                return RedirectToPage("Index");

            var obj = TempData["EditDistributorId"];
            if (obj == null)
                return RedirectToPage("Index");

            int id = (int)obj;
            var distributor = _service.Read(id);
            if (distributor == null)
                return RedirectToPage("Index");

            Distributor = distributor;
            return Page();
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

            _service.Update(Distributor);
            return RedirectToPage("Index");
        }
    }
}