using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly CategoryService _service;

        [BindProperty]
        public Category Category { get; set; } = new();

        public CreateModel()
        {
            _service = new CategoryService(new CategoryRepository());
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Create(Category);
            return RedirectToPage("Index");
        }
    }
}
