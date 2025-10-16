using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class ClientCreator 
{
    private readonly IClientRepository _clientRepository;

    public ClientCreator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    public IClientService FactoryMethod()
    {
        return new ClientService(_clientRepository);
    }
}
