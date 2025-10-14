using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
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

        [TempData]
        public int EditProductId { get; set; }

        public EditModel()
        {
            var creator = new ProductCreator();
            _repository = creator.FactoryMethod();
            _categoryRepository = new CategoryRepository();
        }

        public IActionResult OnGet()
        {
            if (EditProductId == 0)
                return RedirectToPage("Index");

            Product = _repository.Read(EditProductId);

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
            
            // Agregar opción por defecto
            Categories.Insert(0, new SelectListItem { Value = "", Text = "Seleccionar categoría..." });
        }
    }
}