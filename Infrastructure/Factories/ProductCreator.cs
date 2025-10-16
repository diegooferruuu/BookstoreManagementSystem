using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class ProductCreator
{
    public ProductService FactoryMethod()
    {
        return new ProductService(new ProductRepository(), new CategoryRepository());
    }
}
