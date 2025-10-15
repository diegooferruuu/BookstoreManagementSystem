using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Infrastructure.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Application.Ports;


namespace BookstoreManagementSystem.Pages.Distributors
{
    public class IndexModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        public List<Distributor> Distributors { get; set; } = new();

        public IndexModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.GetRepository();
        }

        public void OnGet()
        {
            Distributors = _repository.GetAll();
        }

        public IActionResult OnPostEdit(int id)
        {
            TempData["EditDistributorId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(int id)
        {
            _repository.Delete(id); 
            return RedirectToPage();
        }
    }
}