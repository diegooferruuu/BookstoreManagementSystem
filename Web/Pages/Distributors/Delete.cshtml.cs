using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class DeleteModel : PageModel
    {
        private readonly DistributorService _service;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public DeleteModel(DistributorService service)
        {
            _service = service;
        }

        public IActionResult OnGet(Guid id)
        {
            Distributor = _service.Read(id);
            if (Distributor == null)
                return RedirectToPage("Index");
            return Page();
        }

        public IActionResult OnPost()
        {
            _service.Delete(Distributor.Id);
            return RedirectToPage("Index");
        }
    }
}