using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly CategoryService _service;

        public List<Category> Categories { get; set; } = new();

        public IndexModel()
        {
            _service = new CategoryService(new CategoryRepository());
        }

        public void OnGet()
        {
            Categories = _service.GetAll();
        }
    }
}
