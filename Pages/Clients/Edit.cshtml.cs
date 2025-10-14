using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        [BindProperty]
        public Client Client { get; set; } = new();

        [TempData]
        public int EditClientId { get; set; }

        public EditModel()
        {
            var creator = new ClientCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditClientId"))
                return RedirectToPage("Index");

            int id = (int)TempData["EditClientId"];
            Client = _repository.Read(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _repository.Update(Client);
            return RedirectToPage("Index");
        }
    }
}