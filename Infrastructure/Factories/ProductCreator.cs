using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Application.Ports;

namespace BookstoreManagementSystem.Infrastructure.Factories;

public class ProductCreator : Creator<Product>
{
    public override IDataBase<Product> FactoryMethod()
    {
        return new ProductRepository();
    }
}
