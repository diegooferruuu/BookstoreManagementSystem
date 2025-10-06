using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class CreateModel : PageModel
    {
       
        [BindProperty]
        public Client Cliente { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var repo = new ClientRepository();
            repo.Create(Cliente);
            return RedirectToPage("Index");
        }

    }
}
