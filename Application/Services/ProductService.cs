using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreManagementSystem.Application.Services
{
    public class ProductService : IService<Product>
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        public List<Product> GetAll() => _repository.GetAll();

        public Product? Read(int id) => _repository.Read(id);

        public void Create(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Create(product);
        }

        public void Update(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Update(product);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
