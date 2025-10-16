using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreManagementSystem.Domain.Models;

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
            _service = new ProductService(new ProductRepository(), new CategoryRepository());
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

            // Validación personalizada si la tienes
            // foreach (var err in ProductValidation.Validate(Product, _categoryService))
            //     ModelState.AddModelError($"Product.{err.Field}", err.Message);

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            _service.Create(Product);
            return RedirectToPage("/Products/Index");
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