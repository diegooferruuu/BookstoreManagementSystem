using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;

public class ProductCreator : Creator<Product>
{
    public override IDataBase<Product> FactoryMethod()
    {
        return new ProductRepository();
    }
}
