using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;
using BookstoreManagementSystem.Application.Services;

public class ClientCreator 
{
    public ClientService FactoryMethod()
    {
        return new ClientService(new ClientRepository());
    }
}
