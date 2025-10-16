using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Services
{
    public class CategoryService : IService<Category>
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public List<Category> GetAll() => _repository.GetAll();

        public Category? Read(int id) => _repository.Read(id);

        public void Create(Category category)
        {
            // Validación básica: el nombre no puede estar vacío
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new System.ArgumentException("El nombre de la categoria no puede estar vacio.");
            }
            _repository.Create(category);
        }

        public void Update(Category category)
        {
            // Validación básica: el nombre no puede estar vacío
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new System.ArgumentException("El nombre de la categoria no puede estar vacio.");
            }
            _repository.Update(category);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
