using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;


namespace BookstoreManagementSystem.Pages.Distributors
{
    public class IndexModel : PageModel
    {
        private readonly IDataBase<Distributor> _repository;

        public List<Distributor> Distributors { get; set; } = new();

        public IndexModel()
        {
            var creator = new DistributorCreator();
            _repository = creator.FactoryMethod();
        }

        public void OnGet()
        {
            Distributors = _repository.GetAll();
        }
    }
}