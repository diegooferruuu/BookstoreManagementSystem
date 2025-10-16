using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProductService _service;

        [BindProperty]
        public Product Product { get; set; } = new();

        public DeleteModel()
        {
            _service = new ProductService(new ProductRepository());
        }

        public IActionResult OnGet(int id)
        {
            Product = _service.Read(id);
            if (Product == null)
                return NotFound();
            return Page();
        }

        public IActionResult OnPost()
        {
            _service.Delete(Product.Id);
            return RedirectToPage("/Products/Index");
        }
    }
}