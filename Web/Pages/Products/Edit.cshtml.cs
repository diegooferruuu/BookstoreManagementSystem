using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly ProductService _service;
        private readonly CategoryService _categoryService;

        [BindProperty]
        public Product Product { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        [TempData]
        public int EditProductId { get; set; }

        public EditModel()
        {
            _service = new ProductService(new ProductRepository(), new CategoryRepository());
            _categoryService = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnGet()
        {
            if (EditProductId == 0)
                return RedirectToPage("Index");

            Product = _service.Read(EditProductId);

            if (Product == null)
                return RedirectToPage("Index");

            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            _service.Update(Product);
            return RedirectToPage("/Products/Index");
        }

        private void LoadCategories()
        {
            var categories = _categoryService.GetAll();
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == Product.Category_id
            }).ToList();
            Categories.Insert(0, new SelectListItem { Value = "", Text = "Seleccionar categoría..." });
        }
    }
}