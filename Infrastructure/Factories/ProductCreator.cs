using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

public class ProductCreator : Creator<Product>
{
    public override IDataBase<Product> FactoryMethod()
    {
        return new ProductRepository();
    }
}
