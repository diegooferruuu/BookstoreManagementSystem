using ServiceClients.Domain.Models;
using ServiceClients.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly IClientService _service;

        public List<Client> Clients { get; set; } = new();

        public IndexModel(IClientService service)
        {
            _service = service;
        }

        public void OnGet()
        {
            Clients = _service.GetAll();
        }

        public IActionResult OnPostEdit(Guid id)
        {
            TempData["EditClientId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(Guid id)
        {
            _service.Delete(id);
            return RedirectToPage();
        }
    }
}
