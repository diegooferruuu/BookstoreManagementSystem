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
        public Guid EditProductId { get; set; }

        public EditModel()
        {
            _service = new ProductService(new ProductRepository(), new Infrastructure.Repositories.CategoryRepository());
            _categoryService = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnGet()
        {
            var obj = TempData["EditProductId"];
            if (obj == null)
                return RedirectToPage("Index");

            Guid id;
            if (obj is Guid g)
                id = g;
            else if (obj is string s && Guid.TryParse(s, out g))
                id = g;
            else
                return RedirectToPage("Index");

            var product = _service.Read(id);
            if (product == null)
                return RedirectToPage("Index");

            Product = product;
            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            var domainErrors = BookstoreManagementSystem.Domain.Validations.ProductValidation
                .Validate(Product, new Infrastructure.Repositories.CategoryRepository())
                .ToList();
            foreach (var e in domainErrors)
                ModelState.AddModelError($"Product.{e.Field}", e.Message);

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            // Normalizar según validaciones del dominio
            BookstoreManagementSystem.Domain.Validations.ProductValidation.Normalize(Product);

            try
            {
                _service.Update(Product);
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
                Text = c.Name,
                Selected = c.Id == Product.Category_id
            }).ToList();
            Categories.Insert(0, new SelectListItem { Value = "", Text = "Seleccionar categoría..." });
        }
    }
}