using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;


namespace BookstoreManagementSystem.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly IDataBase<Client> _repository;

        public List<Client> Clients { get; set; } = new();

        public IndexModel()
        {
            var creator = new ClientCreator();
            _repository = creator.FactoryMethod();
        }

        public void OnGet()
        {
            Clients = _repository.GetAll();
        }
    }
}