using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly ClientRepository _repo = new ClientRepository();

        [BindProperty]
        public int Id { get; set; }

        public IActionResult OnGet(int id)
        {
            Id = id;
            return Page();
        }

        public IActionResult OnPost()
        {
            _repo.Delete(Id);
            return RedirectToPage("Index");
        }

    }
}
