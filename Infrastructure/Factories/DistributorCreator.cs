using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class DistributorCreator
{
    private readonly IDistributorRepository _distributorRepository;

    public DistributorCreator(IDistributorRepository distributorRepository)
    {
        _distributorRepository = distributorRepository;
    }
    public IDistributorService FactoryMethod()
    {
        return new DistributorService(_distributorRepository);
    }
}
