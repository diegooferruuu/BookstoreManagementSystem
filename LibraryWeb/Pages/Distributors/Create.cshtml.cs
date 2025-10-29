using ServiceDistributors.Domain.Models;
using ServiceDistributors.Domain.Interfaces;
using ServiceDistributors.Domain.Validations;
using ServiceCommon.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Distributors
{
    public class CreateModel : PageModel
    {
        private readonly IDistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public CreateModel(IDistributorService service)
        {
            _service = service;
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
