using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using BookstoreManagementSystem.Validations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreManagementSystem.Services;

namespace BookstoreManagementSystem.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IDataBase<Product> _repository;
        private readonly CategoryRepository _categoryRepository;

        [BindProperty]
        public Product Product { get; set; } = new();
        
        public List<SelectListItem> Categories { get; set; } = new();

        public EditModel()
        {
            var creator = new ProductCreator();
            _repository = creator.FactoryMethod();
            _categoryRepository = new CategoryRepository();
        }

        public IActionResult OnGet(int id)
        {
            var product = _repository.Read(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;
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

            _repository.Update(Product);
            return RedirectToPage("/Products/Index");
        }

        private void LoadCategories()
        {
            var categories = _categoryRepository.GetAll();
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