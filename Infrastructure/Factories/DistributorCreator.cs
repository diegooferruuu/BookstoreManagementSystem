using BookstoreManagementSystem.Infrastructure.Repositories;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

public class DistributorCreator : Creator<Distributor>
{
    public override IDataBase<Distributor> FactoryMethod()
    {
        return new DistributorRepository();
    }
}
