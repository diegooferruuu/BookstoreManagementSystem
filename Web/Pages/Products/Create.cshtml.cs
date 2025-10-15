using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;
using BookstoreManagementSystem.Domain.Validations;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreManagementSystem.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IDataBase<Product> _repository;
        private readonly CategoryRepository _categoryRepository;

        [BindProperty]
        public Product Product { get; set; } = new();
        
        public List<SelectListItem> Categories { get; set; } = new();

        public CreateModel()
        {
            var creator = new ProductCreator();
            _repository = creator.FactoryMethod();
            _categoryRepository = new CategoryRepository();
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

            foreach (var err in ProductValidation.Validate(Product, _categoryRepository))
                ModelState.AddModelError($"Product.{err.Field}", err.Message);

            if (!ModelState.IsValid)
            {
                LoadCategories();
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($" Campo: {error.Key} - Error: {subError.ErrorMessage}");
                    }
                }
                return Page();
            }

            _repository.Create(Product);
            return RedirectToPage("/Products/Index");
        }

        private void LoadCategories()
        {
            var categories = _categoryRepository.GetAll();
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            
            // Agregar opción por defecto
            Categories.Insert(0, new SelectListItem { Value = "", Text = "Select category..." });
        }
    }
}