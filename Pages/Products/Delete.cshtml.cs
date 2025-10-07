using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Services;

namespace BookstoreManagementSystem.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly IDataBase<Product> _repository;

        [BindProperty]
        public Product Product { get; set; } = new();

        public DeleteModel()
        {
            var creator = new ProductCreator();
            _repository = creator.FactoryMethod();
        }

        public IActionResult OnGet(int id)
        {
            Product = _repository.Read(id);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            _repository.Delete(Product.Id);
            return RedirectToPage("/Products/Index");
        }
    }
}