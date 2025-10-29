using ServiceDistributors.Domain.Models;
using ServiceDistributors.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Distributors
{
    public class IndexModel : PageModel
    {
        private readonly IDistributorService _service;

        public List<Distributor> Distributors { get; set; } = new();

        public IndexModel(IDistributorService service)
        {
            _service = service;
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
