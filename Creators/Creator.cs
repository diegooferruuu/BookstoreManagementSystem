using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;

public abstract class Creator<T>
{
    public abstract IDataBase<T> FactoryMethod();

    public IDataBase<T> GetRepository()
    {
        return FactoryMethod();
    }
}
