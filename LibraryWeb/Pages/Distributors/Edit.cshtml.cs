using ServiceDistributors.Domain.Models;
using ServiceDistributors.Domain.Interfaces;
using ServiceDistributors.Domain.Validations;
using ServiceCommon.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Distributors
{
    public class EditModel : PageModel
    {
        private readonly IDistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        [TempData]
        public Guid EditDistributorId { get; set; }

        public EditModel(IDistributorService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            var obj = TempData["EditDistributorId"];
            if (obj == null)
                return RedirectToPage("Index");

            Guid id;
            if (obj is Guid g)
                id = g;
            else if (obj is string s && Guid.TryParse(s, out g))
                id = g;
            else
                return RedirectToPage("Index");

            var distributor = _service.Read(id);
            if (distributor == null)
                return RedirectToPage("Index");

            Distributor = distributor;
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (var err in DistributorValidation.Validate(Distributor))
                ModelState.AddModelError($"Distributor.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _service.Update(Distributor);
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
