using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;


namespace BookstoreManagementSystem.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        [BindProperty]
        public Client Client { get; set; } = new();

        public DeleteModel()
        {
            var creator = new ClientCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet(int id)
        {
            Client = _repository.Read(id);

            if (Client == null)
                return RedirectToPage("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            _repository.Delete(Client.Id);
            return RedirectToPage("Index");
        }
    }
}