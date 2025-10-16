using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public List<Product> GetAll() => _repository.GetAll();
        public Product? Read(int id) => _repository.Read(id);
        public void Create(Product product) => _repository.Create(product);
        public void Update(Product product) => _repository.Update(product);
        public void Delete(int id) => _repository.Delete(id);
    }
}
