using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ClientRepository _repo = new ClientRepository();

        [BindProperty]
        public Client Cliente { get; set; }

        public IActionResult OnGet(int id)
        {
            Cliente = _repo.Read(id);
            if (Cliente == null)
                return RedirectToPage("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            _repo.Update(Cliente);
            return RedirectToPage("Index");
        }

    }
}
