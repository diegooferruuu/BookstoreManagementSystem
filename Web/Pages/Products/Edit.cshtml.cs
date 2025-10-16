using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;

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
            _service = new ProductService(new ProductRepository());
            _categoryService = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnGet()
        {
            if (EditProductId == 0)
                return RedirectToPage("Index");

            var product = _service.Read(EditProductId);

            if (product == null)
                return RedirectToPage("Index");

            Product = product;

            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            // Ejecutar validaciones del Domain
            foreach (var err in ProductValidation.Validate(Product, new Infrastructure.Repositories.CategoryRepository()))
                ModelState.AddModelError($"Product.{err.Field}", err.Message);

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            // Normalizar
            ProductValidation.Normalize(Product);

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