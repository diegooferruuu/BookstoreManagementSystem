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

        [TempData]
        public int EditDistributorId { get; set; }


        public EditModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet()
        {
            if (EditDistributorId == 0)
                return RedirectToPage("Index");

            Distributor = _repository.Read(EditDistributorId);

            if (Distributor == null)
                return RedirectToPage("Index");

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