using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductService _service;

        public List<Product> Products { get; set; } = new();

        public IndexModel()
        {
            _service = new ProductService(new ProductRepository());
        }

        public void OnGet()
        {
            Products = _service.GetAll();
        }

        public IActionResult OnPostEdit(int id)
        {
            TempData["EditProductId"] = id;
            return RedirectToPage("Edit");
        }

        public IActionResult OnPostDelete(int id)
        {
            _service.Delete(id);
            return RedirectToPage();
        }
    }
}