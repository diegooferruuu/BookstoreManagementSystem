using BookstoreManagementSystem.Application.Ports;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Infrastructure.Repositories;

namespace BookstoreManagementSystem.Infrastructure.Factories;

public class CategoryCreator : Creator<Category>
{
    public override IDataBase<Category> FactoryMethod()
    {
        return new CategoryRepository();
    }
}