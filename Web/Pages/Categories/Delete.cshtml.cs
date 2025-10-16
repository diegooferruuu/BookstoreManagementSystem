using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly CategoryService _service;

        [BindProperty]
        public Category Category { get; set; } = new();

        public DeleteModel()
        {
            _service = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnGet(Guid id)
        {
            Category = _service.Read(id);
            if (Category == null)
                return RedirectToPage("Index");
            return Page();
        }

        public IActionResult OnPost()
        {
            _service.Delete(Category.Id);
            return RedirectToPage("Index");
        }
    }
}
