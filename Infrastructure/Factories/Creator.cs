using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Services;

public abstract class Creator<T>
{
    public abstract IDataBase<T> FactoryMethod();

    public IDataBase<T> GetRepository()
    {
        return FactoryMethod();
    }
}
