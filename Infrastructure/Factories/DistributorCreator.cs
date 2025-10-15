using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;

public class DistributorCreator
{
    public DistributorService FactoryMethod()
    {
        return new DistributorService(new DistributorRepository());
    }
}
