using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class EditModel : PageModel
    {
        private readonly DistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        [TempData]
        public Guid EditDistributorId { get; set; }

        public EditModel()
        {
            _service = new DistributorService(new DistributorRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditDistributorId"))
                return RedirectToPage("Index");

            Guid id = (Guid)TempData["EditDistributorId"];
            Distributor = _service.Read(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Update(Distributor);
            return RedirectToPage("Index");
        }
    }
}