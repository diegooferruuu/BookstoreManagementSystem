using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using BookstoreManagementSystem.Domain.Validations;

namespace BookstoreManagementSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly BookstoreManagementSystem.Domain.Interfaces.ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository repository, BookstoreManagementSystem.Domain.Interfaces.ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        public List<Product> GetAll() => _repository.GetAll();
        public Product? Read(Guid id) => _repository.Read(id);

        public BookstoreManagementSystem.Domain.Interfaces.Result<bool> CreateResult(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository).ToList();
            if (errors.Any())
            {
                var summary = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Message}"));
                return BookstoreManagementSystem.Domain.Interfaces.Result<bool>.Fail(summary);
            }

            ProductValidation.Normalize(product);
            _repository.Create(product);
            return BookstoreManagementSystem.Domain.Interfaces.Result<bool>.Ok(true);
        }

        public BookstoreManagementSystem.Domain.Interfaces.Result<bool> UpdateResult(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository).ToList();
            if (errors.Any())
            {
                var summary = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Message}"));
                return BookstoreManagementSystem.Domain.Interfaces.Result<bool>.Fail(summary);
            }

            ProductValidation.Normalize(product);
            _repository.Update(product);
            return BookstoreManagementSystem.Domain.Interfaces.Result<bool>.Ok(true);
        }

        public void Create(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            ProductValidation.Normalize(product);
            _repository.Create(product);
        }

        public void Update(Product product)
        {
            var errors = ProductValidation.Validate(product, _categoryRepository);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            ProductValidation.Normalize(product);
            _repository.Update(product);
        }

        public void Delete(Guid id) => _repository.Delete(id);
    }
}
