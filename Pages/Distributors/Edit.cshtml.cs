using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;

namespace BookstoreManagementSystem.Pages.Distributors
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        [BindProperty]
        public Distributor Distributor { get; set; } = new();

        public EditModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet(int id)
        {
            var distributor = _repository.Read(id);

            if (distributor == null)
                return RedirectToPage("Index");

            this.Distributor = distributor;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _repository.Update(Distributor);
            return RedirectToPage("Index");
        }
    }
}