using BookstoreManagementSystem.Domain.Models;


namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IClientRepository
    {
        void Create(Client client);
        Client? Read(int id);
        void Update(Client client);
        void Delete(int id);
        List<Client> GetAll();

    }
}
