using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

public class ClientCreator : Creator<Client>
{
    public override IDataBase<Client> FactoryMethod()
    {
        return new ClientRepository();
    }
}
