using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class ProductCreator
{
    private readonly IProductRepository _productRepository;

    public ProductCreator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public IProductService FactoryMethod()
    {
        return new ProductService(_productRepository);
    }
}
