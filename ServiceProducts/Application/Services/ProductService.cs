using ServiceProducts.Domain.Interfaces;
using ServiceProducts.Domain.Models;
using ServiceProducts.Domain.Validations;
using ServiceCommon.Application.Services;

namespace ServiceProducts.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        public List<Product> GetAll() => _repository.GetAll();
        public Product? Read(Guid id) => _repository.Read(id);

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
