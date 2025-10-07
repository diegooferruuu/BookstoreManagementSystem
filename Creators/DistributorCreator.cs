
using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;

public class DistributorCreator : Creator<Distributor>
{
    public override IDataBase<Distributor> FactoryMethod()
    {
        return new DistributorRepository();
    }
}
