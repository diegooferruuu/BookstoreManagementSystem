using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public List<Category> GetAll() => _repository.GetAll();
        public Category? Read(int id) => _repository.Read(id);
        public void Create(Category category) => _repository.Create(category);
        public void Update(Category category) => _repository.Update(category);
        public void Delete(int id) => _repository.Delete(id);
    }
}
