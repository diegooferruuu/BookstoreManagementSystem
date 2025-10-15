using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Application.Ports;

namespace BookstoreManagementSystem.Infrastructure.Factories;

public class DistributorCreator : Creator<Distributor>
{
    public override IDataBase<Distributor> FactoryMethod()
    {
        return new DistributorRepository();
    }
}
