using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly ProductService _service;
        private readonly CategoryService _categoryService;

        [BindProperty]
        public Product Product { get; set; } = new();
        
        public List<SelectListItem> Categories { get; set; } = new();

        public CreateModel()
        {
            _service = new ProductService(new ProductRepository(), new Infrastructure.Repositories.CategoryRepository());
            _categoryService = new CategoryService(new CategoryRepository());
        }

        public void OnGet()
        {
            LoadCategories();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }
            // Ejecutar validaciones del Domain
            foreach (var err in ProductValidation.Validate(Product, new Infrastructure.Repositories.CategoryRepository()))
                ModelState.AddModelError($"Product.{err.Field}", err.Message);

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            // Normalizar antes de persistir según contrato del Domain
            ProductValidation.Normalize(Product);

            try
            {
                _service.Create(Product);
                return RedirectToPage("/Products/Index");
            }
            catch (ValidationException vex)
            {
                foreach (var e in vex.Errors)
                    ModelState.AddModelError($"Product.{e.Field}", e.Message);
                LoadCategories();
                return Page();
            }
        }

        private void LoadCategories()
        {
            var categories = _categoryService.GetAll();
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            Categories.Insert(0, new SelectListItem { Value = "", Text = "Selecciona una categoria..." });
        }
    }
}