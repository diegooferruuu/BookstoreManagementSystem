using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class IndexModel : PageModel
    {
        private readonly DistributorService _service;

        public List<Distributor> Distributors { get; set; } = new();

        public IndexModel()
        {
            _service = new DistributorService(new DistributorRepository());
        }

        public void OnGet()
        {
            Distributors = _service.GetAll();
        }

        public IActionResult OnPostEdit(Guid id)
        {
            TempData["EditDistributorId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(Guid id)
        {
            _service.Delete(id);
            return RedirectToPage();
        }
    }
}