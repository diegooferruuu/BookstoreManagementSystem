using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface IClientService
    {
        public List<Client> GetAll() ;
        public Client? Read(Guid id);
        public void Create(Client client);
        public void Update(Client client);
        public void Delete(Guid id);

    }
}
