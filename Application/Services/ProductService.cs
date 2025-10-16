using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

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

        public void Create(Product product)
        {
            var errors = BookstoreManagementSystem.Domain.Validations.ProductValidation.Validate(product, _categoryRepository);
            if (errors != null && System.Linq.Enumerable.Any(errors))
                throw new ValidationException(errors);

            BookstoreManagementSystem.Domain.Validations.ProductValidation.Normalize(product);
            _repository.Create(product);
        }

        public void Update(Product product)
        {
            var errors = BookstoreManagementSystem.Domain.Validations.ProductValidation.Validate(product, _categoryRepository);
            if (errors != null && System.Linq.Enumerable.Any(errors))
                throw new ValidationException(errors);

            BookstoreManagementSystem.Domain.Validations.ProductValidation.Normalize(product);
            _repository.Update(product);
        }

        public void Delete(Guid id) => _repository.Delete(id);
    }
}
