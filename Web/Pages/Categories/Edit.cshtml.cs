using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly CategoryService _service;

        [BindProperty]
        public Category Category { get; set; } = new();

        [TempData]
        public int EditCategoryId { get; set; }

        public EditModel()
        {
            _service = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnGet()
        {
            if (!TempData.ContainsKey("EditCategoryId"))
                return RedirectToPage("Index");

            int id = (int)TempData["EditCategoryId"];
            Category = _service.Read(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Update(Category);
            return RedirectToPage("Index");
        }
    }
}
