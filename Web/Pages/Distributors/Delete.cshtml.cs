using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;


namespace BookstoreManagementSystem.Pages.Distributors
{
    public class DeleteModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public DeleteModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet(int id)
        {
            Distributor = _repository.Read(id);

            if (Distributor == null)
                return RedirectToPage("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            _repository.Delete(Distributor.Id);
            return RedirectToPage("Index");
        }
    }
}