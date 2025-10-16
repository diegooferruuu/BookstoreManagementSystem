using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class ProductCreator
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductCreator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    public IProductService FactoryMethod()
    {
        return new ProductService(_productRepository, _categoryRepository);
    }
}
