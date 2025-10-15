using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Application.Ports;

namespace BookstoreManagementSystem.Infrastructure.Factories;

public class ClientCreator : Creator<Client>
{
    public override IDataBase<Client> FactoryMethod()
    {
        return new ClientRepository();
    }
}
