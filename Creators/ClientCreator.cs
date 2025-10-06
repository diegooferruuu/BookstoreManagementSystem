using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;

public class ClientCreator : Creator<Client>
{
    public override IDataBase<Client> FactoryMethod()
    {
        return new ClientRepository();
    }
}
