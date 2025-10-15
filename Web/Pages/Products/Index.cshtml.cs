using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Infrastructure.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Application.Ports;

namespace BookstoreManagementSystem.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IDataBase<Product> _repository;

        public List<Product> Products { get; set; } = new();


        public IndexModel()
        {
            var creator = new ProductCreator();
            _repository = creator.GetRepository();
        }

        public void OnGet()
        {
            Products = _repository.GetAll();
        }

        public IActionResult OnPostEdit(int id)
        {
            TempData["EditProductId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(int id)
        {
            _repository.Delete(id); // eliminaci�n l�gica
            return RedirectToPage();
        }
    }
}