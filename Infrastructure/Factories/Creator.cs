using BookstoreManagementSystem.Application.Ports;

namespace BookstoreManagementSystem.Infrastructure.Factories;

public abstract class Creator<T>
{
    public abstract IDataBase<T> FactoryMethod();

    public IDataBase<T> GetRepository()
    {
        return FactoryMethod();
    }
}
